﻿using Org.Reddragonit.BpmEngine.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace Org.Reddragonit.BpmEngine.Elements.Processes.Conditions
{
    [RequiredAttribute("id")]
    internal abstract class ACompareCondition : ACondition
    {
        public ACompareCondition(XmlElement elem, XmlPrefixMap map)
            : base(elem, map) { }

        protected int _Compare(ProcessVariablesContainer variables)
        {
            int ret = -1;
            object left = null;
            if (_GetAttributeValue("leftVariable") != null)
                left = variables[_GetAttributeValue("leftVariable")];
            else
            {
                if (SubNodes != null)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            if (n.Name == "left")
                            {
                                left = n.InnerText;
                                break;
                            }
                        }
                    }
                }
            }
            object right = null;
            if (_GetAttributeValue("rightVariable") != null)
                right = variables[_GetAttributeValue("rightVariable")];
            else
            {
                if (SubNodes != null)
                {
                    foreach (XmlNode n in SubNodes)
                    {
                        if (n.NodeType == XmlNodeType.Element)
                        {
                            if (n.Name == "right")
                            {
                                right = n.InnerText;
                                break;
                            }
                        }
                    }
                }
            }
            if (left == null && right != null)
                ret = -1;
            else if (left != null && right == null)
                return 1;
            else
            {
                if (left is string && right is string)
                    ret = ((string)left).CompareTo(right);
                else
                {
                    if (left is string && !(right is string))
                        left = _ConvertToType((string)left, right.GetType());
                    else if (!(left is string) && right is string)
                        right = _ConvertToType((string)right, left.GetType());
                    else
                        return left.ToString().CompareTo(right.ToString());
                    return ((IComparable)left).CompareTo(right);
                }
            }
            return ret;
        }

        private object _ConvertToType(string value, Type type)
        {
            object ret = value;
            switch (type.FullName)
            {
                case "System.Boolean":
                    ret = bool.Parse(value);
                    break;
                case "System.Byte[]":
                    ret = Convert.FromBase64String(value);
                    break;
                case "System.Char":
                    ret = value[0];
                    break;
                case "System.DateTime":
                    ret = DateTime.Parse(value);
                    break;
                case "System.Decimal":
                    ret = decimal.Parse(value);
                    break;
                case "System.Double":
                    ret = double.Parse(value);
                    break;
                case "System.Single":
                    ret = float.Parse(value);
                    break;
                case "System.Int32":
                    ret = int.Parse(value);
                    break;
                case "System.Int64":
                    ret = long.Parse(value);
                    break;
                case "System.Int16":
                    ret = short.Parse(value);
                    break;
            }
            return ret;
        }

        public override bool IsValid(out string err)
        {
            bool foundLeft = this._GetAttributeValue("leftVariable")!=null;
            bool foundRight = this._GetAttributeValue("rightVariable")!=null;
            foreach (XmlNode n in SubNodes)
            {
                if (n.NodeType == XmlNodeType.Element)
                {
                    switch (n.Name)
                    {
                        case "left":
                            if (foundLeft)
                            {
                                err = "Left value specified more than once.";
                                return false;
                            }
                            foundLeft = true;
                            break;
                        case "right":
                            if (foundRight)
                            {
                                err = "Right value specified more than once.";
                                return false;
                            }
                            foundRight = true;
                            break;
                    }
                }
            }
            if (!foundRight && !foundLeft)
            {
                err = "Right and Left value missing.";
                return false;
            }
            else if (!foundRight)
            {
                err = "Right value missing.";
                return false;
            }
            else if (!foundLeft)
            {
                err = "Left value missing.";
                return false;
            }
            return base.IsValid(out err);
        }
    }
}
