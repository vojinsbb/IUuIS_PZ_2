using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace NetworkService.Model
{
    public class Filter
    {
        public int Id { get; set; } = 0;
        public string Type { get; set; } = null;

        public EntityTypes TypeEnum { get; set; } = EntityTypes.All;
        public string Operation { get; set; } = string.Empty;

        public Filter() 
        {

        }

        public bool FilterEntity(Entity en)
        {
            // Filtriranje po tipu
            if (this.TypeEnum != EntityTypes.All && en.Type.Type != this.TypeEnum)
                return false;

            // Filtriranje po ID i operator
            if (Id != 0)
            {
                switch (Operation)
                {
                    case "Higher":
                        if (!(en.Id > Id)) return false;
                        break;
                    case "Lower":
                        if (!(en.Id < Id)) return false;
                        break;
                    case "Equal":
                        if (!(en.Id == Id)) return false;
                        break;
                }
            }

            return true;
        }

        public string GetName()
        {
            string retValue = "";
            if (Operation != String.Empty)
            {
                string id = Id.ToString();
                switch (Operation)
                {
                    case "Higher":
                        retValue += "ID > " + id + " ";
                        break;
                    case "Lower":
                        retValue += "ID < " + id + " ";
                        break;
                    case "Equal":
                        retValue += "ID = " + id + " ";
                        break;
                    default:
                        retValue = "Error";
                        break;
                }
            }

            if (Type != null)
            {
                if (Type == Model.EntityTypes.Wind_Generator.ToString())
                {
                    retValue += "Type:WG";
                }
                else if (Type == Model.EntityTypes.Solar_Panel.ToString())
                {
                    retValue += "Type:SP";
                }
            }

            return retValue;
        }
    }
}
