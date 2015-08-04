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
    using System.Collections.Generic;
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
    
    #line 1 "..\..\Views\EventStore\MigrationError.cshtml"
    using Newtonsoft.Json;
    
    #line default
    #line hidden
    
    #line 2 "..\..\Views\EventStore\MigrationError.cshtml"
    using Newtonsoft.Json.Linq;
    
    #line default
    #line hidden
    
    #line 3 "..\..\Views\EventStore\MigrationError.cshtml"
    using Sourcery;
    
    #line default
    #line hidden
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("RazorGenerator", "1.2.0.0")]
    [System.Web.WebPages.PageVirtualPathAttribute("~/Views/EventStore/MigrationError.cshtml")]
    public class MigrationError : System.Web.Mvc.WebViewPage<UpdateException>
    {
        public MigrationError()
        {
        }
        public override void Execute()
        {




            
            #line 4 "..\..\Views\EventStore\MigrationError.cshtml"
  
    ViewBag.Title = "title";
    Layout = "~/Views/Shared/_Layout.cshtml";
    


            
            #line default
            #line hidden

WriteLiteral("<h2>There were some migration errors</h2>\r\n");


            
            #line 11 "..\..\Views\EventStore\MigrationError.cshtml"
  

            
            #line default
            #line hidden
WriteLiteral("    <table class=\"zebra-striped\">\r\n");


            
            #line 13 "..\..\Views\EventStore\MigrationError.cshtml"
         foreach (var ce in Model.CommandExceptions)
        {

            
            #line default
            #line hidden
WriteLiteral("            <tr>\r\n                <td>\r\n                    <h3>");


            
            #line 17 "..\..\Views\EventStore\MigrationError.cshtml"
                   Write(ce.Name);

            
            #line default
            #line hidden
WriteLiteral("\r\n");


            
            #line 18 "..\..\Views\EventStore\MigrationError.cshtml"
                         if (ce.Command != null)
                        {
                            
            
            #line default
            #line hidden
            
            #line 20 "..\..\Views\EventStore\MigrationError.cshtml"
                       Write(ce.Command.Timestamp.ToString("f"));

            
            #line default
            #line hidden
            
            #line 20 "..\..\Views\EventStore\MigrationError.cshtml"
                                                               
                        }

            
            #line default
            #line hidden
WriteLiteral("                    </h3>\r\n                    <small title=\"");


            
            #line 23 "..\..\Views\EventStore\MigrationError.cshtml"
                             Write(ce.InnerException.ToString());

            
            #line default
            #line hidden
WriteLiteral("\">");


            
            #line 23 "..\..\Views\EventStore\MigrationError.cshtml"
                                                            Write(ce.InnerException.Message);

            
            #line default
            #line hidden
WriteLiteral("</small>\r\n                    <pre>\r\n                   \r\n");


            
            #line 26 "..\..\Views\EventStore\MigrationError.cshtml"
                              
                                try
                                {
                                    
            
            #line default
            #line hidden
            
            #line 29 "..\..\Views\EventStore\MigrationError.cshtml"
                               Write(JObject.Parse(ce.Json));

            
            #line default
            #line hidden
            
            #line 29 "..\..\Views\EventStore\MigrationError.cshtml"
                                                           
                                }
                                catch (Exception ex)
                                {
                                    
            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\EventStore\MigrationError.cshtml"
                               Write(ce.Json);

            
            #line default
            #line hidden
            
            #line 33 "..\..\Views\EventStore\MigrationError.cshtml"
                                            
                                    
            
            #line default
            #line hidden
            
            #line 34 "..\..\Views\EventStore\MigrationError.cshtml"
                               Write(ex.ToString());

            
            #line default
            #line hidden
            
            #line 34 "..\..\Views\EventStore\MigrationError.cshtml"
                                                  
                                }
                            

            
            #line default
            #line hidden
WriteLiteral("                    </pre>\r\n                </td>\r\n                <td>\r\n        " +
"            <form action=\"");


            
            #line 40 "..\..\Views\EventStore\MigrationError.cshtml"
                             Write(Url.Action("Delete", "EventStore", new { name = ce.Name }));

            
            #line default
            #line hidden
WriteLiteral("\" method=\"POST\">\r\n                    <input type=\"submit\" class=\"btn\" value=\"Del" +
"ete Command\" />\r\n                    </form>\r\n                </td>\r\n           " +
" </tr>\r\n");


            
            #line 45 "..\..\Views\EventStore\MigrationError.cshtml"
        }

            
            #line default
            #line hidden
WriteLiteral("    </table>\r\n");


            
            #line 47 "..\..\Views\EventStore\MigrationError.cshtml"


            
            #line default
            #line hidden

        }
    }
}
#pragma warning restore 1591