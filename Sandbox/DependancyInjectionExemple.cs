using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sandbox
{
    internal class DependancyInjectionExemple
    {
        public Guid ExempleOfASyncCall()
        {
            return Guid.NewGuid();
        }

        public async Task<Guid> ExempleOfAnAsyncCall()
        {
            await Task.Delay(1000);
            return Guid.NewGuid();
        }
    }
}
