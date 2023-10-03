using System.ComponentModel.DataAnnotations;

namespace ShipmentAssistant.Models
{
    public class Item
    {
        public int Index { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        [Range(0, int.MaxValue)]
        public int Units { get; set; }
        [Range(0,int.MaxValue)]
        public int UnitsRecived { get; set; }
        public string Info { get; set; }

        public Item()
        {

        }

        public Item(int index, string code, string name, int units)
        {
            Index = index;
            Code = code;
            Name = name;
            Units = units;
        }

        public override string ToString()
        {
            return $"{Code} {Name} {Units}";
        }
    }
}
