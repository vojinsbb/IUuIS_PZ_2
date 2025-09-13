using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Entity : ClassINotifyPropertyChanged
    {
        private int id;
        private string name;
        private EntityType type;
        private float value;

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
                    Type.Type = value.Type;
                    Type.ImageSource = value.ImageSource;
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
    }
}
