using System;
using System.Linq;
using System.Collections.Generic;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class DesignTimeRealtorWorldUnitOfWork : IRealtorWorldUnitOfWork {
        static IHomeRepository homeRepository = new DesignTimeHomeRepository();
        static IHomePhotoRepository homePhotoRepository = new DesignTimeHomePhotoRepository();
        static IAgentRepository agentRepository = new DesignTimeAgentRepository();
        static IHomeLayoutRepository homeLayoutRepository = new DesignTimeHomeLayoutRepository();
        static IMortgageRateRepository mortgageRateRepository = new DesignTimeMortgageRateRepository();
        static IHomePopularityRatingRepository homePopularityRatingsRepository = new HomePopularityRatingRepository();
        static IHomePriceStatisticDataRepository homePriceStatisticDataRepository = new HomePriceStatisticDataRepository();
        static ISimilarHousesStatisticDataRepository similarHousesStatisticDataRepository = new SimilarHousesStatisticDataRepository();
        static IAgentStatisticDataRepository agentStatisticDataRepository = new AgentStatisticDataRepository();
        
        IHomeRepository IRealtorWorldUnitOfWork.Homes { get { return homeRepository; } }
        IHomePhotoRepository IRealtorWorldUnitOfWork.HomePhotos { get { return homePhotoRepository; } }
        IAgentRepository IRealtorWorldUnitOfWork.Agents { get { return agentRepository; } }
        IHomeLayoutRepository IRealtorWorldUnitOfWork.HomeLayouts { get { return homeLayoutRepository; } }
        IMortgageRateRepository IRealtorWorldUnitOfWork.MortgageRates { get { return mortgageRateRepository; } }
        IHomePopularityRatingRepository IRealtorWorldUnitOfWork.HomePopularityRatings { get { return homePopularityRatingsRepository; } }
        IHomePriceStatisticDataRepository IRealtorWorldUnitOfWork.HomePriceStatisticData { get { return homePriceStatisticDataRepository; } }
        ISimilarHousesStatisticDataRepository IRealtorWorldUnitOfWork.SimilarHousesStatisticData { get { return similarHousesStatisticDataRepository; } }
        IAgentStatisticDataRepository IRealtorWorldUnitOfWork.AgentStatisticData { get { return agentStatisticDataRepository; } }
    }
}
