using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ForecastConnection
{
    /// <summary>
    /// <c>FException</c> исключение выбрасываемое <c>FCommand</c> и <c>FConnection</c>
    /// </summary>
    public class FException : Exception
    {
      public FException() { }
      public FException(string message ) : base( message ) { }
      public FException(string message, Exception inner) : base(message, inner) { }
    }
}
