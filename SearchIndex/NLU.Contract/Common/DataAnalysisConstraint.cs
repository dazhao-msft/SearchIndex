//-----------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  All rights reserved.
//-----------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.BizQA.NLU.Contract
{
    public class DataAnalysisConstraint
    {
        [JsonConstructor]
        public DataAnalysisConstraint(DataOperator dataOperator, DataAnalysisConstraintValue constraintValue)
        {
            DataOperator = dataOperator;
            ConstraintValue = constraintValue;
        }

        /// <summary>
        /// NLExpression operation, Select, Filter,Rank, etc.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public DataOperator DataOperator { get; set; }

        /// <summary>
        /// NLExpression Constraint type
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public DataAnalysisConstraintValue ConstraintValue { get; set; }
    }
}
