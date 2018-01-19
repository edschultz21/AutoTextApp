using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Microsoft.AnalysisServices.AdomdClient;
using System.Runtime.Serialization.Formatters.Binary;

namespace TenK.InfoSparks.Common.AnalysisServices
{
    [Serializable]
    [DataContract]
    [KnownType(typeof(AxisItem))]
    [KnownType(typeof(ResultAxis))]
    [KnownType(typeof(Object[]))]
    public class MDXQueryResult
    {
        [DataContract]
        public class AxisItem
        {
            #region Equality members

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = PositionNum;
                    hashCode = (hashCode*397) ^ (UniqueName != null ? UniqueName.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (Caption != null ? Caption.GetHashCode() : 0);
                    return hashCode;
                }
            }

            #endregion

            #region Equality members

            protected bool Equals(AxisItem other)
            {
                return PositionNum == other.PositionNum && string.Equals(UniqueName, other.UniqueName) && string.Equals(Caption, other.Caption);
            }

            #endregion

            [DataMember]
            public readonly int PositionNum;
            [DataMember]
            public readonly string UniqueName;
            [DataMember]
            public readonly string Caption;
            [DataMember]
            public readonly string LevelName;

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((AxisItem) obj);
            }

            public AxisItem(int position, string uniqueName, string caption)
            {
                PositionNum = position;
                UniqueName = uniqueName;
                Caption = caption;
                LevelName = "";
            }

            public AxisItem(int position, string uniqueName, string caption, string levelName)
            {
                PositionNum = position;
                UniqueName = uniqueName;
                Caption = caption;
                LevelName = levelName;
            }
        }

        [DataContract]
        public class ResultAxis
        {
            #region Equality members

            public override int GetHashCode()
            {
                unchecked
                {
                    int hashCode = AxisNum;
                    hashCode = (hashCode*397) ^ (AxisName != null ? AxisName.GetHashCode() : 0);
                    hashCode = (hashCode*397) ^ (AxisItems != null ? AxisItems.GetHashCode() : 0);
                    return hashCode;
                }
            }

            #endregion

            [DataMember]
            public int AxisNum;
            [DataMember]
            public string AxisName;
            [DataMember]
            public AxisItem[] AxisItems;

            public ResultAxis(int axisNum, string axisName, int numPositions)
            {
                AxisNum = axisNum;
                AxisName = axisName;
                AxisItems = new AxisItem[numPositions];
            }
            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj)) return false;
                if (ReferenceEquals(this, obj)) return true;
                if (obj.GetType() != this.GetType()) return false;
                return Equals((ResultAxis) obj);
            }

            #region Equality members

            protected bool Equals(ResultAxis other)
            {
                bool result =  AxisNum == other.AxisNum && string.Equals(AxisName, other.AxisName);
                result &= AxisItems.Length == other.AxisItems.Length;

                if (result)
                {
                    for (int i = 0; i < AxisItems.Length; i++)
                    {
                        result &= AxisItems[i].Equals(other.AxisItems[i]);
                    }
                }
                return result;
            }

            #endregion
        }

        // Query Result Identification Information
        [DataMember]
        public ResultAxis[] ResultAxes;
        [DataMember]
        public object[] DataNodes;

        public MDXQueryResult()
        {
        }

        public MDXQueryResult(CellSet resultCs, bool includeFirstOnly = false)
        {
            CloneFromCellSet(resultCs, includeFirstOnly);
        }

        public MDXQueryResult(int firstAxisCount)
        {
            // Create first axis
            ResultAxes = new ResultAxis[1] { new ResultAxis(0,"na",firstAxisCount)
                                {
                                    AxisItems = new AxisItem[firstAxisCount]
                                }
            };
            DataNodes = new object[firstAxisCount];
        }

        public MDXQueryResult(MDXQueryResult original)
        {
            DataNodes = original.DataNodes;
            ResultAxes = original.ResultAxes;
        }

        private void CloneFromCellSet(CellSet resultCS, bool includeFirstOnly = false)
        {
            int totalAxisCount = includeFirstOnly ? 1 : resultCS.Axes.Count;
            ResultAxes = new ResultAxis[totalAxisCount];
            
            int dataElementCount = 1;

            for (int i = 0; i < totalAxisCount; i++)
            {
                // TODO: Add some logic here to throw exception if results arent what we expect
                if (resultCS.Axes[i].Positions.Count > 0)
                {
                    ResultAxes[i] = new ResultAxis(i, resultCS.Axes[i].Positions[0].Members[0].LevelName, resultCS.Axes[i].Positions.Count);
                }
                else
                {
                    ResultAxes[i] = new ResultAxis(i, resultCS.Axes[i].Name, resultCS.Axes[i].Positions.Count);
                    dataElementCount *= 0;
                }

                // Calculate how much space we will need to store all the data
                dataElementCount *= ResultAxes[i].AxisItems.Length;

                for (int j = 0; j < resultCS.Axes[i].Positions.Count; j++)
                {
                    string caption = resultCS.Axes[i].Positions[j].Members[0].Caption;
                    string uniqueName = resultCS.Axes[i].Positions[j].Members[0].UniqueName;
                    string levelName = resultCS.Axes[i].Positions[j].Members[0].LevelName;

                    for (int k = 1; k < resultCS.Axes[i].Positions[j].Members.Count; k++)
                    {
                        var member = resultCS.Axes[i].Positions[j].Members[k];
                        caption += "," + member.Caption;
                        uniqueName += "," + member.UniqueName;
                        levelName += "," + member.LevelName;
                    }

                    ResultAxes[i].AxisItems[j] = new AxisItem(j, uniqueName, caption, levelName);
                }
            }

            DataNodes = new object[dataElementCount];

            // initialize data array
            InitalizeArrayLevel(DataNodes, resultCS, 0, new int[0]);

        }

        // initialize data array and fill data
        private void InitalizeArrayLevel(object[] dataNodes, CellSet resultCS, int axisNum, int[] coordinates)
        {
            //dataNodes = new Node[resultCS.Axes[axisNum].Positions.Count];

            for (int i = 0; i < resultCS.Axes[axisNum].Positions.Count; i++)
            {
                int[] newcoords = new int[coordinates.Length + 1];
                coordinates.CopyTo(newcoords, 0);
                newcoords[newcoords.Length - 1] = i;

                if (axisNum != resultCS.Axes.Count - 1)
                {
                    InitalizeArrayLevel(dataNodes, resultCS, axisNum + 1, newcoords);                 
                }
                else // leaf level, populate data
                {
                    dataNodes[CalculateOffset(newcoords)] = resultCS.Cells[newcoords].Value == null ? null : resultCS.Cells[newcoords].Value;
                }
            }
        }

        private int CalculateOffset(IList<int> coordinates)
        {
            int runningTotal = 0;
            int multiplier = 1;

            for (int i = ResultAxes.Length -1; i >= 0 ; i--)
            {
                runningTotal += (i < coordinates.Count ? coordinates[i] : 0) * multiplier;
                multiplier *= ResultAxes[i].AxisItems.Length;
            }
            return runningTotal;
        }

        private int CalculateLength(int[] coordinates)
        {
            int resultLength = 1;
            int length = coordinates.Length;
            for (int i = 0; i < ResultAxes.Length; i++)
            {
                resultLength *= (i < length ? 1 : ResultAxes[i].AxisItems.Length);
            }
            return resultLength;
        }

        public override bool Equals(object o)
        {
            if (ReferenceEquals(null, o)) return false;
            if (ReferenceEquals(this, o)) return true;
            if (o.GetType() != this.GetType()) return false;
            return Equals((MDXQueryResult) o);
        }

        protected bool Equals(MDXQueryResult other)
        {
            bool result = true;

            // Compare Axes
            for (int i = 0; i < ResultAxes.Length; i++)
            {
                result &= ResultAxes[i].Equals(other.ResultAxes[i]);
            }

            // Compare Data
            result &= TraverseCompareData(other);
            return result;
        }

        private bool TraverseCompareData(MDXQueryResult other)
        {
            bool result = DataNodes.Length == other.DataNodes.Length;
            if (result)
            {
                for (int i = 0; i < DataNodes.Length; i++)
                {
                    result &= DataNodes[i] == other.DataNodes[i];
                }
            }
            return result;
        }

        public double? GetData(params int[] coords)
        {
            object datum = DataNodes[CalculateOffset(coords)];
            return datum == null || !(datum is double) ? (double?)null : Convert.ToDouble(datum);
        }

        public object Get(params int[] coords)
        {
            return DataNodes[CalculateOffset(coords)];
        }

        /*
        public Object GetData(int axis0)
        {
            return DataNodes[axis0];
        }

        public Object GetData(int axis0, int axis1)
        {
            return ((Object[])DataNodes[axis0])[axis1];
        }

        public Object GetData(int axis0, int axis1, int axis2)
        {
            return ((Object[])((Object[])DataNodes[axis0])[axis1])[axis2];
        }

        public Object GetData(int axis0, int axis1, int axis2, int axis3)
        {
            return ((Object[])((Object[])((Object[])DataNodes[axis0])[axis1])[axis2])[axis3];
        }*/

        private static MDXQueryResult DeserializeQueryResult(byte[] inData, BinaryFormatter bf)
        {
            MemoryStream cims = new MemoryStream(inData);
            return (MDXQueryResult)(bf.Deserialize(cims));
        }

        public static MDXQueryResult DeserializeQueryResult(byte[] inData)
        {
            BinaryFormatter bf = new BinaryFormatter();
            return DeserializeQueryResult(inData, bf);
        }

        public static byte[]  SerializeQueryResult(MDXQueryResult result)
        {
            BinaryFormatter bf = new BinaryFormatter();
            MemoryStream res = new MemoryStream();
            bf.Serialize(res,result);
            return res.ToArray();
        }

        public double?[] GetPartialData(int[] coordinates)
        {
            
            var startPosition = CalculateOffset(coordinates);
            var length = CalculateLength(coordinates);
            var result = new double?[length];
            Array.Copy(DataNodes,startPosition,result,0,length);
            return result;
        }
    }
}
