using System;

namespace DevExpress.RealtorWorld.Xpf.DataModel {
    public class RealtorWorldUnitOfWork : IRealtorWorldUnitOfWork {
        IHomeRepository homeRepository;
        IHomePhotoRepository homePhotoRepository;
        IAgentRepository agentRepository;
        IHomeLayoutRepository homeLayoutRepository;
        IMortgageRateRepository mortgageRateRepository;
        IHomePopularityRatingRepository homePopularityRatingsRepository;
        IHomePriceStatisticDataRepository homePriceStatisticDataRepository;
        ISimilarHousesStatisticDataRepository similarHousesStatisticDataRepository;
        IAgentStatisticDataRepository agentStatisticDataRepository;

        IHomeRepository IRealtorWorldUnitOfWork.Homes {
            get {
                if(homeRepository == null)
                    homeRepository = new HomeRepository();
                return homeRepository;
            }
        }
        IHomePhotoRepository IRealtorWorldUnitOfWork.HomePhotos {
            get {
                if(homePhotoRepository == null)
                    homePhotoRepository = new HomePhotoRepository();
                return homePhotoRepository;
            }
        }
        IAgentRepository IRealtorWorldUnitOfWork.Agents {
            get {
                if(agentRepository == null)
                    agentRepository = new AgentRepository();
                return agentRepository;
            }
        }
        IHomeLayoutRepository IRealtorWorldUnitOfWork.HomeLayouts {
            get {
                if(homeLayoutRepository == null)
                    homeLayoutRepository = new HomeLayoutRepository();
                return homeLayoutRepository;
            }
        }
        IMortgageRateRepository IRealtorWorldUnitOfWork.MortgageRates {
            get {
                if(mortgageRateRepository == null)
                    mortgageRateRepository = new MortgageRateRepository();
                return mortgageRateRepository;
            }
        }
        IHomePopularityRatingRepository IRealtorWorldUnitOfWork.HomePopularityRatings {
            get {
                if(homePopularityRatingsRepository == null)
                    homePopularityRatingsRepository = new HomePopularityRatingRepository();
                return homePopularityRatingsRepository;
            }
        }
        IHomePriceStatisticDataRepository IRealtorWorldUnitOfWork.HomePriceStatisticData {
            get {
                if(homePriceStatisticDataRepository == null)
                    homePriceStatisticDataRepository = new HomePriceStatisticDataRepository();
                return homePriceStatisticDataRepository;
            }
        }
        ISimilarHousesStatisticDataRepository IRealtorWorldUnitOfWork.SimilarHousesStatisticData {
            get {
                if(similarHousesStatisticDataRepository == null)
                    similarHousesStatisticDataRepository = new SimilarHousesStatisticDataRepository();
                return similarHousesStatisticDataRepository;
            }
        }
        IAgentStatisticDataRepository IRealtorWorldUnitOfWork.AgentStatisticData {
            get {
                if(agentStatisticDataRepository == null)
                    agentStatisticDataRepository = new AgentStatisticDataRepository();
                return agentStatisticDataRepository;
            }
        }
    }
}
