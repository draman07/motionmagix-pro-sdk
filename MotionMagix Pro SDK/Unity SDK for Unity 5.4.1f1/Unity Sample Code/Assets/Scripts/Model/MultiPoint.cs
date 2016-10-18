using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Xml.Serialization;
using UnityEngine;

namespace MotionMagixSimulator.Model
{
    [Serializable]
    [XmlRoot(ElementName = "packet")]
    public class MultiPoint
    {
        private List<Coordinate> _MultiPointCoordinates;
        [XmlElement(ElementName = "object")]
        public List<Coordinate>  MultiPointCoordinates
        {
            get
            {
                return _MultiPointCoordinates;
            }
            set
            {
                if(_MultiPointCoordinates !=value)
                {
                    _MultiPointCoordinates = value;
                }
            }
        }

        public MultiPoint()
        {
            MultiPointCoordinates = new List<Coordinate>();
        }
    }

	public class PointEventArgs:EventArgs
	{
		public MultiPoint MultiPoint;
	}

	public class SilhouetteEventArgs:EventArgs
	{
		public Texture2D SilhoutteImage;
	}
   
}
