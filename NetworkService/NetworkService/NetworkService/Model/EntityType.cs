using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class EntityType : ClassINotifyPropertyChanged
    {
        private string type;
        private string imageSource = "";

        public string Type
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
