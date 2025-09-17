using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public enum EntityTypes
    {
        Solar_Panel,
        Wind_Generator, 
        All
    }

    public class EntityType : ClassINotifyPropertyChanged
    {
        private EntityTypes type;
        private string imageSource = "";

        public EntityTypes Type
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

        public string ImageSource
        {
            get { return imageSource; }
            set 
            {
                if (imageSource != value)
                {
                    imageSource = value;
                    OnPropertyChanged("ImageSource");
                }
            }
        }
    }
}
