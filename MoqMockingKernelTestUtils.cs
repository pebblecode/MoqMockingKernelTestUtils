using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Moq;
using Ninject.MockingKernel.Moq;
using System.Linq.Expressions;

namespace TestUtils
{
    public class MoqMockingKernelTestUtils
    {
        private MoqMockingKernel _kernel = new MoqMockingKernel();

        /// <summary>
        /// Automatically wires up the indicated repository to return the collection 
        /// of mock/stub objects for all methods of type EListType.  Additionally, 
        /// any methods of type EType will return the first item in the collection
        /// </summary>
        /// <typeparam name="RepositoryType">The repository type to wire up</typeparam>
        /// <typeparam name="EType">BO type of the repository</typeparam>
        /// <typeparam name="EListType">BO collection type</typeparam>
        /// <param name="entitiesToReturn">Mock/stub objects already configured</param>
        /// <returns>The mock repository wrapper for further configuration</returns>
        protected Mock<RepositoryType> AutoWireUpMockRepository<RepositoryType, EType, EListType>(params EType[] entitiesToReturn)
            where RepositoryType : class
            where EListType : List<EType>, new()
        {
            //Use the MockingKernel to get a Mock repository object from the DI container
            Mock<RepositoryType> mockRepo = _kernel.GetMock<RepositoryType>();

            //Search for repo accessor methods
            typeof(RepositoryType).GetMethods()
                .ForEach(mi =>
                {
                    //Build a lamba expression in the format expected by the Moq, e.g.
                    //  mockRepo.Setup(repo => repo.GetByObjectProperties(prop1value, prop2value)

                    //the parameter for the lambda will therefore be a repository
                    var parameter = Expression.Parameter(typeof(RepositoryType));

                    //The body will be a function call on the repo that matches this particular method
                    var body = Expression.Call(parameter, mi,

                        //Since we don't care what parameters are passed (for this simple repo mock)
                        //match each parameter to a generic call of It.IsAny<ParamType>
                        mi.GetParameters().Select(pi =>
                            Expression.Call(typeof(It), "IsAny", new Type[] { pi.ParameterType })));

                    if (mi.ReturnType.Equals(typeof(EListType)))
                    {
                        //For collection types, return all indicated items
                        var lambdaExpression = Expression.Lambda<Func<RepositoryType, EListType>>(body, parameter);
                        EListType newCollection = new EListType();
                        newCollection.AddRange(entitiesToReturn);
                        mockRepo.Setup(lambdaExpression).Returns(newCollection);
                    }
                    else if (mi.ReturnType.Equals(typeof(EType)))
                    {
                        //For accessors returning a single item, pick the first item in the collection
                        var lambdaExpression = Expression.Lambda<Func<RepositoryType, EType>>(body, parameter);
                        mockRepo.Setup(lambdaExpression).Returns(entitiesToReturn.First());
                    }
                });

            return mockRepo;
        }
    }
}
