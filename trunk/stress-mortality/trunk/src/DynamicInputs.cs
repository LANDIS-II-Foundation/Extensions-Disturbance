//  Copyright 2005-2010 Portland State University, University of Wisconsin
//  Authors:  Robert M. Scheller, James B. Domingo

using Landis.Core;
using System.Collections.Generic;
using System.IO;

namespace Landis.Extension.StressMortality
{

    public class DynamicInputs
    {
        private static Dictionary<int, List<IDynamicInputRecord>> allData;
        private static List<IDynamicInputRecord> timestepData;

        public DynamicInputs()
        {
        }

        public static Dictionary<int, List<IDynamicInputRecord>> AllData
        {
            get {
                return allData;
            }
            set
            {
                allData = value;
            }
        }
        //---------------------------------------------------------------------
        public static List<IDynamicInputRecord> TimestepData
        {
            get {
                return timestepData;
            }
            set {
                timestepData = value;
            }
        }

    }

}
