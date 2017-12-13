﻿/*
 * 2017 Sizing Servers Lab
 * University College of West-Flanders, Department GKG
 * 
 */

using SizingServers.Util;
using System;
using System.Collections;
using System.Diagnostics;
using System.Text;

namespace sizingservers.beholderv2.agent.shared {
    /// <summary>
    /// Describes a property of a piece of hardware / component group.
    /// </summary>
    [DebuggerDisplay("Type = {Type}, Name = {Name}, Value = {Value}, UniqueId = {UniqueId}, Unit = {Unit}")]
    public class PayloadProperty {
        /// <summary>
        /// 
        /// </summary>
        public enum PayloadType { Collection, String, Long, Double, Boolean }
        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }
        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public string Value { get; set; }
        /// <summary>
        /// When matching hardware (ComponentGroups) as it moves around, this property is used as a sole identifier if this is true, except if it is null or empty. The other fields are disregarded. This is handy for, for instance a disk moves from Linux to Windows, the model name is formatted differently, but the serial number stays the same.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [unique identifier]; otherwise, <c>false</c>.
        /// </value>
        public bool UniqueId { get; set; }
        /// <summary>
        /// Gets or sets the unit.
        /// </summary>
        /// <value>
        /// The unit.
        /// </value>
        public string Unit { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadProperty"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value. The "ToString()" is stored. OOr if it is an IEnumerable it will be serialized as x0\t ...\t xn\ with the PayloadType String (only single level collections are handled correctly).</param>
        /// <param name="uniqueId">When matching hardware (ComponentGroups) as it moves around, this property is used as a sole identifier if this is true, except if it is null or empty. The other fields are disregarded. This is handy for, for instance a disk moves from Linux to Windows, the model name is formatted differently, but the serial number stays the same.</param>
        /// <param name="unit">%, GB, empty string,... cannot be null</param>
        public PayloadProperty(string name, object value, bool uniqueId = false, string unit = "") {
            Name = name;
            SetValue(value);
            UniqueId = uniqueId;
            Unit = unit;
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="PayloadProperty"/> class.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value. The "ToString()" is stored. Or if it is an IEnumerable it will be serialized as x0\t ...\t xn\ with the PayloadType String (only single level collections are handled correctly).</param>
        /// <param name="uniqueId">When matching hardware (ComponentGroups) as it moves around, this property is used as a sole identifier if this is true. The other fields are disregarded. This is handy for, for instance a disk moves from Linux to Windows, the model name is formatted differently, but the serial number stays the same.</param>
        /// <param name="unit">%, GB, empty string,... cannot be null</param>
        private PayloadProperty(string type, string name, string value, bool uniqueId, string unit) {
            Type = type;
            Name = name;
            Value = value;
            UniqueId = uniqueId;
            Unit = unit;
        }
        /// <summary>
        /// Sets the type safely. Stored as a string for easy serialisation.
        /// </summary>
        /// <param name="type">Type of the payload.</param>
        public void SetType(PayloadType type) { Type = Enum.GetName(typeof(PayloadType), type); }
        /// <summary>
        /// Sets the value. The "ToString()" is stored. Or if it is an IEnumerable it will be serialized as x0\t ...\t xn\ with the PayloadType String (only single level collections are handled correctly).
        /// </summary>
        /// <param name="value">The value.</param>
        public void SetValue(object value) {
            if (value == null) value = "";

            bool set = false;
            if (value is string) {
                SetType(PayloadType.String);
                set = true;
            }
            else if (value is float || value is double || value is decimal) {
                SetType(PayloadType.Double);
                set = true;
            }
            else if (value is short || value is ushort || value is int || value is uint || value is long || value is ulong) {
                SetType(PayloadType.Long);
                set = true;
            }
            else if (value is bool) {
                SetType(PayloadType.Boolean);
                set = true;
            }

            if (!set && value is IEnumerable) {
                SetType(PayloadType.Collection);
                var enumerator = (value as IEnumerable).GetEnumerator();
                enumerator.Reset();

                var sb = new StringBuilder();
                bool firstItem = true;
                while (enumerator.MoveNext()) {
                    if (firstItem)
                        firstItem = false;
                    else
                        sb.Append("\t");

                    sb.Append(ToString(enumerator.Current));
                }

                Value = sb.ToString();
                return;
            }

            Value = ToString(value);
        }

        /// <summary>
        /// Returns a correctly formatted <see cref="System.String" />, no scientific notation.
        /// </summary>
        /// <param name="singleValue">A single value, no IEnumerable.</param>
        /// <returns>
        /// A correctly formatted <see cref="System.String" />, no scientific notation.
        /// </returns>
        private string ToString(object singleValue) {
            if (singleValue is float)
                return StringUtil.FloatToLongString((float)singleValue);
            else if (singleValue is double)
                return StringUtil.DoubleToLongString((double)singleValue);
            else if (singleValue is decimal)
                return StringUtil.DecimalToLongString((decimal)singleValue);

            return singleValue.ToString();
        }

        /// <summary>
        /// Clones this instance.
        /// </summary>
        /// <returns></returns>
        public PayloadProperty Clone() { return new PayloadProperty(Type, Name, Value, UniqueId, Unit); }
    }
}
