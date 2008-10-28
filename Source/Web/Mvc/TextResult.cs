using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Web.Mvc
{
    /// <summary>
    /// ActionResult that renders specified text as a result
    /// </summary>
    public class TextResult : ActionResult
    {
        /// <summary>
        /// Creates a TextResult using <paramref name="text"/>
        /// </summary>
        /// <param name="text"></param>
        public TextResult(string text)
        {
            Text = text;
        }

        /// <summary>
        /// Text to be written in the output
        /// </summary>
        public string Text { get; private set; }
        
        /// <summary>
        /// <see cref="ActionResult.ExecuteResult"/>
        /// </summary>
        /// <param name="context"></param>
        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.Write(Text);
        }
    }
}
