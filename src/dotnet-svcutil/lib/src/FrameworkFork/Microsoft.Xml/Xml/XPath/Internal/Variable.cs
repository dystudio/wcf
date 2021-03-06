// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace MS.Internal.Xml.XPath
{
    using System;
    using Microsoft.Xml;
    using Microsoft.Xml.XPath;

    internal class Variable : AstNode
    {
        private string _localname;
        private string _prefix;

        public Variable(string name, string prefix)
        {
            _localname = name;
            _prefix = prefix;
        }

        public override AstType Type { get { return AstType.Variable; } }
        public override XPathResultType ReturnType { get { return XPathResultType.Any; } }

        public string Localname { get { return _localname; } }
        public string Prefix { get { return _prefix; } }
    }
}
