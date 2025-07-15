using System.Collections.Generic;
using UnityEngine;
using HeneGames.Airplane;
using System.Collections;


namespace Assets.scripts.PlaneController
{
    public class AiPlaneController:SimpleAirPlaneController
    {

        [Header("Gun")]
        [SerializeField] private Transform firePoint;
        [SerializeField] private float bulletDamage = 1f;
        [SerializeField] private float fireDistance = 1300f;        // максимальная дистанция огня
        [SerializeField] private float fireAngleThreshold = 5f;    // точность наведения перед выстрелом
        [SerializeField] private float bulletSpeed = 10f;         // скорость пули
        [SerializeField] private float fireCooldown = 0.02f;       // перезарядка
        [SerializeField] private float lastFireTime = -999f;
        [SerializeField] private float spreadAngle = 2f; // Максимальный разброс в градусах
        [SerializeField] private float maxAimAngle = 15f; // Максимальный угол, при котором разрешено стрелять, но с разбросом
        [SerializeField] private float bulletRadius = 0.5f;
        [SerializeField] private LayerMask shootMask;
        [SerializeField] private float dynamicAngleFactor = 3f;

        // Вспомогательный список для точек попадания лучей
        private List<Vector3> lastHitPoints = new List<Vector3>();

        protected override void Start()
        {
            // Включаем режим ИИ
            isAIPlane = true;
            base.Start();
        }

        protected override void HandleInputs()
        {
            AiHandler();
            Fire();
        }

        protected override void AiHandler()
        {
          
            GameObject playerPlane = GameObject.FindGameObjectWithTag("PlayerPlane");
            if (playerPlane!=null)
                target = playerPlane.transform.position;

            Vector3 desired = ComputeDesiredDirection();

            float yawAngle = Vector3.SignedAngle(transform.forward, desired, transform.up);
            float pitchAngle = Vector3.SignedAngle(transform.forward, desired, transform.right);

            inputH = Mathf.Clamp(yawAngle / controlAngleThreshold, -1f, 1f);
            inputV = Mathf.Clamp(pitchAngle / controlAngleThreshold, -1f, 1f);

            inputYawLeft = yawAngle < -controlAngleThreshold;
            inputYawRight = yawAngle > controlAngleThreshold;

            Vector3 toTarget = (target - transform.position).normalized;
            float forwardDot = Vector3.Dot(transform.forward, toTarget);
            float angleFactor = 1f - Mathf.Clamp01((forwardDot - 0.8f) / 0.2f);
            float desiredTrust = Mathf.Lerp(0.2f, 1f, angleFactor);

            float dist = Vector3.Distance(transform.position, target);
            float distFactor = Mathf.Clamp01(dist / minDistanceForSlowdown);

            desiredTrust *= distFactor;
            trustProcent = Mathf.Clamp01(Mathf.Lerp(trustProcent, desiredTrust, Time.deltaTime));
        }

        protected override Vector3 ComputeDesiredDirection()
        {
            lastHitPoints.Clear();

            if (target == null)
                return transform.forward;

            Vector3 toTarget = (target - transform.position).normalized;

            Vector3[] localDirs = new Vector3[]
            {
                Vector3.forward,
                Quaternion.Euler(0, 30, 0) * Vector3.forward,
                Quaternion.Euler(0, -30, 0) * Vector3.forward,
                Quaternion.Euler(30, 0, 0) * Vector3.forward,
                Quaternion.Euler(-30, 0, 0) * Vector3.forward
            };

            float closestDist = float.MaxValue;
            Vector3 hitNormal = Vector3.zero;
            RaycastHit hitInfo;

            void CastFrom(Vector3 origin)
            {
                foreach (var dirLocal in localDirs)
                {
                    Vector3 dirWorld = transform.TransformDirection(dirLocal);
                    Debug.DrawRay(origin, dirWorld * detectDistance, Color.red);
                    if (Physics.Raycast(origin, dirWorld, out hitInfo, detectDistance, obstacleMask))
                    {
                        lastHitPoints.Add(hitInfo.point);
                        if (hitInfo.distance < closestDist)
                        {
                            closestDist = hitInfo.distance;
                            hitNormal = hitInfo.normal;
                        }
                    }
                }
            }

            Vector3 nose = transform.position + transform.forward * 2f;
            CastFrom(nose);
            CastFrom(nose - transform.right * 5f);
            CastFrom(nose + transform.right * 5f);

            if (closestDist < float.MaxValue)
            {
                Vector3 avoidDir = Vector3.Reflect(transform.forward, hitNormal).normalized;
                float t = Mathf.Clamp01(1f - (closestDist / detectDistance));
                return Vector3.Slerp(avoidDir, toTarget, 1f - t).normalized;
            }

            return toTarget;
        }

        protected override void Fire()
        {
            if (Time.time - lastFireTime < fireCooldown|| fireDistance<Vector3.Distance(transform.position,target)) return;

            Vector3 dir = firePoint.forward;
            dir = AddSpreadToDirection(dir, spreadAngle);
            Vector3 startPos = firePoint.position;
            Vector3 endPos = startPos + dir * fireDistance;

            if (Physics.Raycast(startPos, dir, out RaycastHit hit, fireDistance, shootMask))
            {
                endPos = hit.point;
                var stats = hit.collider.GetComponentInParent<PlaneStats>();
                if (stats != null)
                    stats.TakeDamage(bulletDamage);
            }

            StartCoroutine(ShowBulletTrail(firePoint.position, endPos));
            lastFireTime = Time.time;
        }


        private Vector3 AddSpreadToDirection(Vector3 direction, float angleInDegrees)
        {
            float angleRad = angleInDegrees * Mathf.Deg2Rad;
            Vector3 randomDir = Random.insideUnitSphere;

            Vector3 axis = Vector3.Cross(direction, randomDir).normalized;
            float randomAngle = Random.Range(0f, angleInDegrees);

            Quaternion rotation = Quaternion.AngleAxis(randomAngle, axis);
            return rotation * direction;
        }

        private IEnumerator ShowBulletTrail(Vector3 start, Vector3 end)
        {
            GameObject lineObj = new GameObject("BulletTrail");
            LineRenderer lr = lineObj.AddComponent<LineRenderer>();

            lr.startWidth = 0.2f;
            lr.endWidth = 0.2f;
            lr.material = new Material(Shader.Find("Unlit/Color"));
            lr.material.color = Color.yellow;

            lr.SetPosition(0, start);
            lr.SetPosition(1, end);

            yield return new WaitForSeconds(0.3f);

            Destroy(lineObj);
        }
    }
    
}
