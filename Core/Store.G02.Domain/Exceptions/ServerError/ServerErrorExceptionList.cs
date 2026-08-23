using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.G02.Domain.Exceptions.ServerError
{
    public class ServerErrorExceptionList(IEnumerable<string> Errors) :
        ServerErrorException(string.Join(", ", Errors))
    {
    }
}
