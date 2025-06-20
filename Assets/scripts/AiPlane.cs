using HeneGames.Airplane;
using System.Collections.Generic;
using UnityEngine;

public class AiPlane : SimpleAirPlaneController
{
    [Header("AI Settings")]
    [Tooltip("Target Transform (e.g. Player)")]
    public Transform target;

    [Tooltip("Layers considered obstacles (including player if desired)")]
    public LayerMask obstacleMask = ~0;

    [Tooltip("Distance ahead to check for obstacles")]
    public float detectDistance = 40f;

    [Tooltip("Maximum angle (deg) used to normalize control inputs")]
    public float controlAngleThreshold = 45f;

    // Для визуализации точек пересечения
    private List<Vector3> lastHitPoints = new List<Vector3>();

    protected override float GetInputHorizontal()
    {
        Vector3 dir = ComputeDesiredDirection();
        float yawAngle = Vector3.SignedAngle(transform.forward, dir, Vector3.up);
        return Mathf.Clamp(yawAngle / controlAngleThreshold, -1f, 1f);
    }

    protected override float GetInputVertical()
    {
        Vector3 dir = ComputeDesiredDirection();
        float pitchAngle = Vector3.SignedAngle(transform.forward, dir, transform.right);
        return Mathf.Clamp(pitchAngle / controlAngleThreshold, -1f, 1f);
    }

    protected override bool GetInputTurbo()
    {
        return true;
    }

    protected override bool GetInputYawLeft() => false;
    protected override bool GetInputYawRight() => false;
    protected override float GetTrustDelta() => 0f;

    /// <summary>
    /// Расчёт желаемого направления: уклонение от препятствий и погоня за целью.
    /// Также сохраняет точки столкновения для Gizmos.
    /// </summary>
    private Vector3 ComputeDesiredDirection()
    {
        lastHitPoints.Clear();

        if (target == null)
            return transform.forward;

        // Направление к цели
        Vector3 toTarget = (target.position - transform.position).normalized;

        // Точки старта лучей: нос, крылья
        Vector3 nosePos = transform.position + transform.forward * 2f;
        Vector3 leftWing = transform.position - transform.right * 5f;
        Vector3 rightWing = transform.position + transform.right * 5f;

        // Локальные направления для лучей
        Vector3[] localDirs = new[]
        {
            Vector3.forward,
            Quaternion.Euler(0, 30, 0) * Vector3.forward,
            Quaternion.Euler(0, -30, 0) * Vector3.forward,
            Quaternion.Euler(30, 0, 0) * Vector3.forward,
            Quaternion.Euler(-30, 0, 0) * Vector3.forward,
        };

        float closestDist = float.MaxValue;
        Vector3 hitNormal = Vector3.zero;

        RaycastHit hitInfo;
        System.Action<Vector3> CastFrom = origin =>
        {
            foreach (var ld in localDirs)
            {
                Vector3 dir = transform.TransformDirection(ld);
                if (Physics.Raycast(origin, dir, out hitInfo, detectDistance, obstacleMask))
                {
                    // Save hit point for Gizmos
                    lastHitPoints.Add(hitInfo.point);

                    if (hitInfo.distance < closestDist)
                    {
                        closestDist = hitInfo.distance;
                        hitNormal = hitInfo.normal;
                    }
                }
            }
        };

        // Проверяем три позиции
        CastFrom(nosePos);
        CastFrom(leftWing);
        CastFrom(rightWing);

        if (closestDist < float.MaxValue)
        {
            Vector3 avoidDir = Vector3.Reflect(transform.forward, hitNormal).normalized;
            float t = Mathf.Clamp01(1f - (closestDist / detectDistance));
            Vector3 mixed = Vector3.Slerp(avoidDir, toTarget, 1f - t).normalized;

            // Ограничиваем крутой тангаж: проецируем на плоскость локальной правой оси
            Vector3 horizontal = Vector3.ProjectOnPlane(mixed, transform.right).normalized;
            return horizontal;
        }

        return toTarget;
    }

    private void OnDrawGizmosSelected()
    {
        if (lastHitPoints == null || lastHitPoints.Count == 0)
            return;

        Gizmos.color = Color.red;
        foreach (var p in lastHitPoints)
        {
            Gizmos.DrawSphere(p, 0.5f);
        }
    }
}
