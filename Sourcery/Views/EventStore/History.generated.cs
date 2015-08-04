﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.488
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sourcery.Views.EventStore
{
    using System;
    
    #line 3 "..\..\Views\EventStore\History.cshtml"
    using System.Collections.Generic;
    
    #line default
    #line hidden
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Web;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Mvc.Ajax;
    using System.Web.Mvc.Html;
    using System.Web.Routing;
    using System.Web.Security;
    using System.Web.UI;
    using System.Web.WebPages;
    
    #line 1 "..\..\Views\EventStore\History.cshtml"
    using Newtonsoft.Json.Linq;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\EventStore\History.cshtml"
    using Sourcery;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.2.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EventStore/History.cshtml")]
    public class History : System.Web.Mvc.WebViewPage<Tuple<int, IEnumerable<Tuple<Command, string>>>>
    {
        public History()
        {
        }
        public override void Execute()
        {




WriteLiteral("<h2>Event History</h2>\r\n");


            
            #line 6 "..\..\Views\EventStore\History.cshtml"
  

            
            #line default
            #line hidden
WriteLiteral(@"    <table class=""zebra-striped bordered-table"">
        <thead>
            <tr>
                <th>Time</th>
                <th>Hostname</th>
                <th>Path</th>
                <th>Action</th>
                <th>User</th>
            </tr>
        </thead>
        <tbody>
");


            
            #line 18 "..\..\Views\EventStore\History.cshtml"
             foreach (var ce in Model.Item2)
            {
                var jo = JObject.Parse(ce.Item2);
                

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td>");


            
            #line 23 "..\..\Views\EventStore\History.cshtml"
                   Write(ce.Item1.Timestamp.ToString("f"));

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n\r\n                    <td>\r\n                        " +
"");


            
            #line 27 "..\..\Views\EventStore\History.cshtml"
                   Write(jo["Hostname"]);

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                    <td>\r\n                        ");


            
            #line 30 "..\..\Views\EventStore\History.cshtml"
                   Write(jo["Path"]);

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                    <td>");


            
            #line 32 "..\..\Views\EventStore\History.cshtml"
                   Write(jo["Name"]);

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                    <td>");


            
            #line 34 "..\..\Views\EventStore\History.cshtml"
                   Write(jo["CurrentUserId"]);

            
            #line default
            #line hidden
WriteLiteral("\r\n                    </td>\r\n                </tr>\r\n");


            
            #line 37 "..\..\Views\EventStore\History.cshtml"
              
                {
                    {

            
            #line default
            #line hidden
WriteLiteral("                <tr>\r\n                    <td colspan=\"5\">\r\n                     " +
"   <pre>");


            
            #line 42 "..\..\Views\EventStore\History.cshtml"
                         Write(jo.ToString());

            
            #line default
            #line hidden
WriteLiteral("</pre>\r\n                    </td>\r\n                </tr> \r\n");


            
            #line 45 "..\..\Views\EventStore\History.cshtml"
                    }
                }

            }

            
            #line default
            #line hidden
WriteLiteral("        </tbody>\r\n    </table>\r\n");



WriteLiteral("    <div class=\"pagination\">\r\n        <ul>");



WriteLiteral("\r\n");


            
            #line 53 "..\..\Views\EventStore\History.cshtml"
             for (int i = 1; (i * 10) < Model.Item1; i++)
            {

            
            #line default
            #line hidden
WriteLiteral("                <li ");


            
            #line 55 "..\..\Views\EventStore\History.cshtml"
                Write((Request.Params["page"] ?? "1") == i.ToString() ? "class=active" : "0");

            
            #line default
            #line hidden
WriteLiteral("><a href=\"/eventstore/history?page=");


            
            #line 55 "..\..\Views\EventStore\History.cshtml"
                                                                                                                           Write(i);

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 55 "..\..\Views\EventStore\History.cshtml"
                                                                                                                               Write(i);

            
            #line default
            #line hidden
WriteLiteral("</a></li>\r\n");


            
            #line 56 "..\..\Views\EventStore\History.cshtml"
            }

            
            #line default
            #line hidden
WriteLiteral("            ");



WriteLiteral(" </ul>\r\n    </div>\r\n");


            
            #line 59 "..\..\Views\EventStore\History.cshtml"


            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591