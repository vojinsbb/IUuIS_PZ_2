using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetworkService.Model;

namespace NetworkService.Model
{
    public class Entity : ClassINotifyPropertyChanged
    {
        private int id;
        private string name;
        private EntityType type;
        private float value;

        public List<float> ValueHistory { get; set; } = new List<float>();
        public List<string> TimelineValues { get; set; } = new List<string>();

        public Entity(int id, string name, EntityTypes entityTypeEnum, float value)
        {
            this.id = id;
            this.name = name;
            this.value = value;

            // Napravi EntityType objekat iz enum vrednosti
            this.type = new EntityType
            {
                Type = entityTypeEnum,
                ImageSource = GetImageSourceForType(entityTypeEnum)
            };
        }

        private string GetImageSourceForType(EntityTypes type)
        {
            switch (type)
            {
                case EntityTypes.Solar_Panel:
                    return "Images/solar_panel.jpg"; // stavi putanju do slike
                case EntityTypes.Wind_Generator:
                    return "Images/wind_generator.jpg"; // stavi putanju do slike
                default:
                    return "";
            }
        }

        public int Id
        {
            get { return id; }
            set 
            {
                if (id != value)
                {
                    id = value;
                    OnPropertyChanged("Id");
                }
            }
        }

        public string Name
        {
            get { return name; }
            set 
            {
                if (name != value)
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
        }

        public EntityType Type
        {
            get { return type; }
            set
            {
                if (type != value)
                {
                    type = value;
                    OnPropertyChanged("Type");
                }
            }
        }

        public float Value
        {
            get { return value; }
            set 
            {
                if (this.value != value)
                {
                    this.value = value;
                    OnPropertyChanged("Value");
                }
            }
        }

        public bool isValueValid()
        {
            bool retVal = false;

            if(Value >= 1 && Value <= 5)
            {
                retVal = true;
            }

            return retVal;
        }

        public void AddValue(float newValue)
        {
            ValueHistory.Add(newValue);
            TimelineValues.Add(DateTime.Now.ToString());

            if (ValueHistory.Count > 5)
            {
                ValueHistory.RemoveAt(0);
                TimelineValues.RemoveAt(0);
            }

            Value = newValue;
        }
    }
}
