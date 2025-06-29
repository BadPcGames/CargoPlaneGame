using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.scripts.Cargo
{
    public class CargoNamesList
    {
        public List<CargoName> CargoNames;

        public CargoNamesList()
        {
            CargoNames = new List<CargoName>
            {
            new CargoName("Documents", 0.1f),
            new CargoName("Small parcels", 0.2f),
            new CargoName("Books", 0.3f),
            new CargoName("Pharmaceuticals", 0.8f),
            new CargoName("Clothing", 0.5f),
            new CargoName("Shoes", 0.6f),
            new CargoName("Laptops", 1.4f),
            new CargoName("Smartphones", 1.6f),
            new CargoName("Medical equipment", 1.9f),
            new CargoName("Watches", 2.1f),
            new CargoName("Jewelry", 2.5f),
            new CargoName("Industrial tools", 1.2f),
            new CargoName("Engineering parts", 1.0f),
            new CargoName("Glassware", 1.3f),
            new CargoName("Cosmetics", 0.9f),
            new CargoName("Musical instruments", 1.5f),
            new CargoName("Paintings", 2.0f),
            new CargoName("Sculptures", 2.3f),
            new CargoName("Antiques", 2.4f),
            new CargoName("Wine bottles", 1.1f),
            new CargoName("Cigars", 1.0f),
            new CargoName("Perishable food", 0.7f),
            new CargoName("Frozen seafood", 0.8f),
            new CargoName("Luxury handbags", 2.2f),
            new CargoName("Perfume", 1.7f),
            new CargoName("Televisions", 1.8f),
            new CargoName("Monitors", 1.3f),
            new CargoName("Gaming consoles", 1.6f),
            new CargoName("Electrical components", 1.2f),
            new CargoName("Car parts", 1.4f),
            new CargoName("Motorcycles", 2.0f),
            new CargoName("Bicycles", 1.1f),
            new CargoName("Sports equipment", 1.0f),
            new CargoName("Camping gear", 0.9f),
            new CargoName("Drones", 1.9f),
            new CargoName("Camera equipment", 2.0f),
            new CargoName("Film reels", 2.1f),
            new CargoName("Server hardware", 2.3f),
            new CargoName("Solar panels", 1.8f),
            new CargoName("Laser equipment", 2.4f),
            new CargoName("Chemical samples", 1.5f),
            new CargoName("Research specimens", 2.2f),
            new CargoName("Fine fabrics", 1.3f),
            new CargoName("Leather goods", 1.4f),
            new CargoName("Plastic pellets", 0.6f),
            new CargoName("Rubber materials", 0.7f),
            new CargoName("Steel rods", 1.1f),
            new CargoName("Aluminum sheets", 1.0f),
            new CargoName("Copper wire", 1.2f),
            new CargoName("Titanium parts", 2.0f),
            new CargoName("Gold bars", 2.5f),
            new CargoName("Silver coins", 2.3f),
            new CargoName("Luxury watches", 2.4f),
            new CargoName("VR headsets", 1.7f),
            new CargoName("Robotics kits", 1.6f),
            new CargoName("Lab instruments", 2.0f),
            new CargoName("Satellite components", 2.5f),
            new CargoName("Aircraft parts", 2.2f),
            new CargoName("Flight recorders", 2.1f),
            new CargoName("Navigation systems", 1.9f),
            new CargoName("Radar modules", 2.3f),
            new CargoName("3D printers", 1.8f),
            new CargoName("Smart home devices", 1.4f),
            new CargoName("Medical implants", 2.0f),
            new CargoName("Dental tools", 1.5f),
            new CargoName("Hearing aids", 1.6f),
            new CargoName("Glasses & lenses", 0.8f),
            new CargoName("Seeds", 0.5f),
            new CargoName("Live plants", 0.6f),
            new CargoName("Cultural artifacts", 2.4f),
            new CargoName("Drone batteries", 1.7f),
            new CargoName("EV chargers", 1.9f),
            new CargoName("Luxury furniture", 2.2f),
            new CargoName("Mini fridges", 1.3f),
            new CargoName("Kitchen appliances", 1.1f),
            new CargoName("Vacuum cleaners", 1.0f),
            new CargoName("Power tools", 1.2f),
            new CargoName("Baby products", 0.9f),
            new CargoName("Strollers", 1.1f),
            new CargoName("Diapers", 0.4f),
            new CargoName("Toys", 0.6f),
            new CargoName("Board games", 0.5f),
            new CargoName("Educational kits", 0.8f),
            new CargoName("First-aid kits", 1.0f),
            new CargoName("Survival gear", 1.3f),
            new CargoName("Outdoor drones", 2.0f),
            new CargoName("Hunting optics", 1.5f),
            new CargoName("Binoculars", 1.6f),
            new CargoName("Compact safes", 1.8f),
            new CargoName("Security cameras", 1.9f),
            new CargoName("Smart locks", 1.7f),
            new CargoName("Networking gear", 1.4f),
            new CargoName("Fire extinguishers", 0.9f),
            new CargoName("Vacuum tubes", 1.3f),
            new CargoName("Hard drives", 1.5f),
            new CargoName("SSD storage", 1.8f),
            new CargoName("Graphic cards", 2.3f),
            new CargoName("CPUs", 2.2f),
            new CargoName("Motherboards", 2.0f),
            new CargoName("RAM modules", 1.6f),
            new CargoName("Cooling systems", 1.2f),
            new CargoName("Server racks", 2.1f)
            };
        }
    }

    public class CargoName
    {
        public string Name { get; set; }
        public float ValueKof { get;set; }

        public CargoName(string name,float kof)
        {
            Name = name;
            ValueKof = kof;
        }
    }
}
