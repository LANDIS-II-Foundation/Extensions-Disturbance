//  Copyright 2005-2009 University of Wisconsin, Portland State University
//  Authors:  Jimm Domingo, Robert M. Scheller
//  License:  Available at
//  http://www.landis-ii.org/developers/LANDIS-IISourceCodeLicenseAgreement.pdf

using Edu.Wisc.Forest.Flel.Util;

namespace Landis.Wind
{
    public class EventParameters
        : IEventParameters
    {
        private double maxSize;
        private double meanSize;
        private double minSize;
        private int rotationPeriod;

        //---------------------------------------------------------------------

        /// <summary>
        /// Maximum event size (hectares).
        /// </summary>
        public double MaxSize
        {
            get {
                return maxSize;
            }
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                if (meanSize > 0.0 && value < meanSize)
                    throw new InputValueException(value.ToString(), "Value must be = or > MeanSize.");
                if (minSize > 0.0 && value < minSize)
                    throw new InputValueException(value.ToString(),"Value must be = or > MinSize.");
                maxSize = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Mean event size (hectares).
        /// </summary>
        public double MeanSize
        {
            get {
                return meanSize;
            }
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                if (maxSize > 0.0 && value > maxSize)
                    throw new InputValueException(value.ToString(), "Value must be < or = MaxSize.");
                if (minSize > 0.0 && value < minSize)
                    throw new InputValueException(value.ToString(), "Value must be = or > MinSize.");
                meanSize = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Minimum event size (hectares).
        /// </summary>
        public double MinSize
        {
            get {
                return minSize;
            }
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                if (meanSize > 0.0 && value > meanSize)
                    throw new InputValueException(value.ToString(), "Value must be < or = MeanSize.");
                if (maxSize > 0.0 && value > maxSize)
                    throw new InputValueException(value.ToString(), "Value must be < or = MaxSize.");
                minSize = value;
            }
        }

        //---------------------------------------------------------------------

        /// <summary>
        /// Wind rotation period (years).
        /// </summary>
        public int RotationPeriod
        {
            get {
                return rotationPeriod;
            }
            set {
                if (value < 0)
                    throw new InputValueException(value.ToString(), "Value must be = or > 0.");
                rotationPeriod = value;
            }
        }

        //---------------------------------------------------------------------

        public EventParameters()
        {
        }
        //---------------------------------------------------------------------
/*
        public EventParameters(double maxSize,
                               double meanSize,
                               double minSize,
                               int rotationPeriod)
        {
            this.maxSize = maxSize;
            this.meanSize = meanSize;
            this.minSize = minSize;
            this.rotationPeriod = rotationPeriod;
        }

        //---------------------------------------------------------------------

        public EventParameters()
        {
            this.maxSize = 0.0;
            this.meanSize = 0.0;
            this.minSize = 0.0;
            this.rotationPeriod = 0;
        }*/
    }
}
