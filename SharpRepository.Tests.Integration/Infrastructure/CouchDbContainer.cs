using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpRepository.Tests.Integration.Infrastructure
{
    public class CouchDbContainer : ContainerBase
    {
        public CouchDbContainer() 
            : base("couchdb", "1.7.2", "sr-couchdb", 5984) { }
    }
}
