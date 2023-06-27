using OSS.Data.Entities;

namespace OSS.Services.Specification
{
    public class TestTableWithDescSpecification : BaseSpecification<TestTable>
    {
        public TestTableWithDescSpecification(string term) : base(x => x.Text1.Contains(term) || x.Text2.Contains(term))
        {
                
        }
    }
}
