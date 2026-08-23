using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Domain.Exceptions.ServerError
{
    public class ServerErrorException(string message) : Exception(message)
    {
    }
}
