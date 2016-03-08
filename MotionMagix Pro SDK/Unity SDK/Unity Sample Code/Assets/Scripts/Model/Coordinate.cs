using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace MotionMagixSimulator.Model
{
    [Serializable]    
    public class Coordinate
    {
        private int _XCoordinate;
        [XmlElement(ElementName = "x")]
        public int XCoordinate
        {
            get
            {
                return _XCoordinate;
            }
            set
            {
                if(_XCoordinate!=value)
                {
                    _XCoordinate = value;
                }
            }
        }

        private int _yCoordinate;
        [XmlElement(ElementName = "y")]
        public int YCoordinate
        {
            get
            {
                return _yCoordinate;
            }
            set
            {
                if (_yCoordinate != value)
                {
                    _yCoordinate = value;
                }
            }
        }

		public int Life;

        public Coordinate() { }
    }
}
