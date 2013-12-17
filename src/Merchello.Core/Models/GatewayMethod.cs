﻿using System;
using System.Runtime.Serialization;

namespace Merchello.Core.Models
{
    /// <summary>
    /// Defines a GatewayMethod 
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class GatewayMethod : IGatewayMethod
    {
        public GatewayMethod(string serviceCode, string name)
        {
            Mandate.ParameterNotNullOrEmpty(serviceCode, "serviceCode");
            Mandate.ParameterNotNullOrEmpty(name, "name");

            ServiceCode = serviceCode;
            Name = name;
        }

        /// <summary>
        /// The unique provider service code or 'alias' for the gateway method.
        /// </summary>
        [DataMember]
        public string ServiceCode { get; private set; }

        /// <summary>
        /// The descriptive name of the Gateway Method
        /// </summary>
        [DataMember]
        public string Name { get; private set; }
    }
}