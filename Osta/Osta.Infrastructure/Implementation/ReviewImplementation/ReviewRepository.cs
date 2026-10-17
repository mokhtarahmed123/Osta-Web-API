using Microsoft.EntityFrameworkCore;
using Osta.Data.Entities;
using Osta.Infrastructure.Abstract.ReviewAbstract;
using Osta.Infrastructure.DataBase;
using Osta.Infrastructure.InfrastructureBases;

namespace Osta.Infrastructure.Implementation.ReviewImplementation
{
    public class ReviewRepository : GenericRepositoryAsync<Review>, IReviewRepository
    {
        #region Vars / Props
        private readonly DbSet<Review> Review;
        #endregion
        #region Constructor(s)
        public ReviewRepository(OstaContext dbContext) : base(dbContext)
        {
            Review = dbContext.Set<Review>();
        }

        #endregion


    }
}
