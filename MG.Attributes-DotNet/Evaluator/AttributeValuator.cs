using System;

namespace MG.Attributes
{
    /// <summary>
    /// A class that can read attributes from given decorated class members.
    /// </summary>
    public partial class AttributeValuator : IAttributeValueReader
    {
        /// <summary>
        /// The default constructor initializing a new <see cref="AttributeValuator"/> instance.
        /// </summary>
        public AttributeValuator() { }
    }
}
