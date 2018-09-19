using NUnit.Framework;
using SharpRepository.Tests.Integration.Data;
using SharpRepository.Tests.Integration.Infrastructure;
using System.Collections.Generic;

namespace SharpRepository.Tests.Integration.TestAttributes
{
    public class ExecuteForAllCompoundKeyRepositoriesAttribute : SourceAttribute
    {
        private static IEnumerable<TestCaseData> ForAllCompoundKeyRepositoriesTestCaseData
        {
            get
            {
                return CompoundKeyRepositoryTestCaseDataFactory.Build(RepositoryTypes.CompoundKey);
            }
        }

        public ExecuteForAllCompoundKeyRepositoriesAttribute()
            : base(typeof(ExecuteForAllCompoundKeyRepositoriesAttribute), "ForAllCompoundKeyRepositoriesTestCaseData")
        {
        }
    }
}