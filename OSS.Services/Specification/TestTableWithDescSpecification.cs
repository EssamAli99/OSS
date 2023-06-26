using OSS.Data.Entities;

namespace OSS.Services.Specification
{
    public class TestTableWithDescSpecification : BaseSpecifcation<TestTable>
    {
        public TestTableWithDescSpecification(string term) : base(x => x.Text1.Contains(term) || x.Text2.Contains(term))
        {
                
        }
    }
}
