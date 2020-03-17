using System;
using AutoTextApp;

namespace UnitTests
{
    public class TestDefinitionProvider : IDefinitionProvider
    {
        public string GetDirectionText(DirectionType direction, bool isIncreasePostive)
        {
            if (direction == DirectionType.FLAT)
            {
                return "stayed flat";
            }
            else if (direction == DirectionType.POSITIVE)
            {
                return "went up";
            }
            else // direction == DirectionType.NEGATIVE
            {
                return "went down";
            }
        }

        public MetricDefinition GetMetricDefinition(string metricCode)
        {
            switch (metricCode.ToUpper())
            {
                case "NL":
                    return new MetricDefinition
                    {
                        Code = "NL",
                        ShortName = "New Listings",
                        LongName = "New Listings LongName",
                        IsPlural = true,
                        IsIncreasePostive = true
                    };
                case "CS":
                    return new MetricDefinition
                    {
                        Code = "CS",
                        ShortName = "Closed Sales",
                        LongName = "Closed Sales LongName",
                        IsPlural = true,
                        IsIncreasePostive = true
                    };
                case "MSP":
                    return new MetricDefinition
                    {
                        Code = "MSP",
                        ShortName = "Median Sales Price",
                        LongName = "MSP LongName",
                        IsPlural = true,
                        IsIncreasePostive = true
                    };
                case "MSI":
                    return new MetricDefinition
                    {
                        Code = "MSI",
                        ShortName = "MSI",
                        LongName = "Months Supply Inventory",
                        IsPlural = true,
                        IsIncreasePostive = false
                    };
                default:
                    throw new Exception($"Metric {metricCode} not found");
            }
        }

        public VariableDefinition GetVariableDefinition(string variableCode)
        {
            switch (variableCode.ToUpper())
            {
                case "SF":
                    return new VariableDefinition
                    {
                        Code = "SF",
                        ShortName = "Single Family",
                        LongName = "Single Family homes"

                    };
                case "TC":
                    return new VariableDefinition
                    {
                        Code = "TC",
                        ShortName = "Townhouse/Condo",
                        LongName = "Townhouse/Condo homes"

                    };
                case "MB":
                    return new VariableDefinition
                    {
                        Code = "MB",
                        ShortName = "Mobile",
                        LongName = "Mobile homes"

                    };
                default:
                    throw new Exception($"Variable {variableCode} not found");
            }
        }
    }

    public class TestDataProvider : IDataProvider
    {
        private VariableData CreateVariableData(int id, float currentValue, float previousValue, float percentChange)
        {
            var direction = DirectionType.FLAT;
            if (percentChange >= 0.05)
            {
                var isPositive = (currentValue - previousValue) > 0;
                direction = isPositive ? DirectionType.POSITIVE : DirectionType.NEGATIVE;
            }

            return new VariableData
            {
                Id = id,
                CurrentValue = currentValue,
                PreviousValue = previousValue,
                PercentChange = percentChange,
                Direction_Old = direction,
                ConsecutivePeriods = 1
            };
        }

        public VariableData GetVariableData(string metricCode, string variableCode)
        {
            var varCode = variableCode?.ToUpper();
            switch (metricCode.ToUpper())
            {
                case "NL":
                    switch (varCode)
                    {
                        case "SF": return CreateVariableData(101, 82.5F, 92.5F, 7.5F);
                        case "TC": return CreateVariableData(102, 92.5F, 82.5F, 7.5F);
                        case "MB": return CreateVariableData(103, 164.7F, 154.8F, 17.5F);
                    }
                    break;
                case "CS":
                    switch (varCode)
                    {
                        case "SF": return CreateVariableData(101, 82.5F, 92.5F, 0.04F);
                        case "TC": return CreateVariableData(102, 92.5F, 82.5F, 0.04F);
                        case "MB": return CreateVariableData(103, 264.7F, 254.8F, 27.5F);
                    }
                    break;
                case "MSP":
                    switch (varCode)
                    {
                        case "SF": return CreateVariableData(101, 82.5F, 92.5F, 0.04F);
                        case "TC": return CreateVariableData(102, 209000F, 200000F, 13.9F);
                        case "MB": return CreateVariableData(103, 364.7F, 354.8F, 37.5F);
                    }
                    break;
                case "MSI":
                    return CreateVariableData(-1, 2.4F, 7.2F, 12.3F);
            }

            return null;
        }
    }
}
