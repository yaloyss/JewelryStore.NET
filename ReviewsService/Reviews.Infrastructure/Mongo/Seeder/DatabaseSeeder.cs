using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using Reviews.Domain.Entities;

namespace Reviews.Infrastructure.Mongo.Seeder
{
    public class DatabaseSeeder : IDataSeeder
    {
        private readonly MongoDbContext _context;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(MongoDbContext context, ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Starting data seeding...");
                await SeedRatingsAsync(cancellationToken);
                await SeedReviewsAsync(cancellationToken);
                await SeedDiscussionsAsync(cancellationToken);
                _logger.LogInformation("Data seeding completed successfully!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during data seeding");
                throw;
            }
        }

        private async Task SeedRatingsAsync(CancellationToken cancellationToken)
        {
            //ідемпотентність
            var existingRatings = await _context.Ratings.Find(_ => true).Limit(1).AnyAsync(cancellationToken);
            if (existingRatings)
            {
                _logger.LogInformation("Ratings already seeded. Skipping...");
                return;
            }

            _logger.LogInformation("Seeding ratings...");

            var ratings = new List<Rating>
            {
                new Rating(5),
                new Rating(5),
                new Rating(5),
                new Rating(5),
                new Rating(5),
                new Rating(4),
                new Rating(4),
                new Rating(3)
            };
            await _context.Ratings.InsertManyAsync(ratings, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully seeded {Count} ratings", ratings.Count);
        }

        private async Task SeedReviewsAsync(CancellationToken cancellationToken)
        {
            var existingReviews = await _context.Reviews.Find(_ => true).Limit(1).AnyAsync(cancellationToken);
            if (existingReviews)
            {
                _logger.LogInformation("Reviews already seeded. Skipping...");
                return;
            }
            _logger.LogInformation("Seeding reviews...");

            //reviews with embedded documents (ratings)
            var reviews = new List<Review>
            {
                new Review(
                    productId: 17,
                    rating: new Rating(5),
                    title: "Such a beautiful necklace!",
                    body: "Just the perfect necklace, buy it and you won't regret!"
                ),
                new Review(
                    productId: 9,
                    rating: new Rating(5),
                    title: "Perfect pendant for everyday wear",
                    body: "Beautiful pendant, goes with everything."
                ),
                new Review(
                    productId: 6,
                    rating: new Rating(5),
                    title: "Anniversary gift",
                    body: "Bought these earrings for my wife for our anniversary, she really liked them."
                ),
                new Review(
                    productId: 15,
                    rating: new Rating(5),
                    title: "Simple and elegant",
                    body: "Very elegant bracelet, I wear it everyday!"
                ),
                new Review(
                    productId: 1,
                    rating: new Rating(5),
                    title: "The perfect ring",
                    body: "My boyfriend proposed to me with this ring, I love it!"
                ),
                new Review(
                    productId: 17,
                    rating: new Rating(4),
                    title: "Good quality",
                    body: "Nice necklace, good quality for the price. Shipping was fast."
                ),
                new Review(
                    productId: 9,
                    rating: new Rating(3),
                    title: "It's okay",
                    body: "The pendant is nice but smaller than I expected. Still wearable though."
                ),
                new Review(
                    productId: 15,
                    rating: new Rating(4),
                    title: "Pretty bracelet",
                    body: "Beautiful design, though the clasp could be sturdier. Overall happy with purchase."
                ),
            };
            await _context.Reviews.InsertManyAsync(reviews, cancellationToken: cancellationToken);
            _logger.LogInformation("Successfully seeded {Count} reviews", reviews.Count);
        }

        private async Task SeedDiscussionsAsync(CancellationToken cancellationToken)
        {
            var existingDiscussions = await _context.Discussions.Find(_ => true).Limit(1).AnyAsync(cancellationToken);
            if (existingDiscussions)
            {
                _logger.LogInformation("Discussions already seeded. Skipping...");
                return;
            }

            //getting reviews to seed discussions
            var reviews = await _context.Reviews.Find(_ => true).Limit(5).ToListAsync(cancellationToken);

            if (!reviews.Any())
            {
                _logger.LogWarning("No reviews found. Cannot seed discussions.");
                return;
            }

            _logger.LogInformation("Seeding discussions...");
            var discussions = new List<Discussion>();

            if (reviews.Count > 0)
            {
                var discussion1 = new Discussion(reviews[0].Id);
                discussion1.AddMessage("Does the necklace feel heavy on the neck?");
                discussion1.AddMessage("No, not at all, don't worry about it!");
                discussions.Add(discussion1);
            }

            if (reviews.Count > 1)
            {
                var discussion2 = new Discussion(reviews[1].Id);
                discussion2.AddMessage("Does it tarnish over time?");
                discussion2.AddMessage("No, I've been wearing it every day and it still looks new.");
                discussion2.AddMessage("Perfect, thanks!");
                discussions.Add(discussion2);
            }

            if (reviews.Count > 2)
            {
                var discussion3 = new Discussion(reviews[2].Id);
                discussion3.AddMessage("Are these earrings suitable for sensitive ears?");
                discussion3.AddMessage("Yes, my wife wears them just fine, no irritation.");
                discussion3.AddMessage("Great, thank you for the reply!");
                discussions.Add(discussion3);
            }

            if (reviews.Count > 3)
            {
                var discussion4 = new Discussion(reviews[3].Id);
                discussion4.AddMessage("Is it easily breakable?");
                discussion4.AddMessage("I wouldn't say so, but I wear it carefully anyway.");
                discussions.Add(discussion4);
            }

            if (reviews.Count > 4)
            {
                var discussion5 = new Discussion(reviews[4].Id);
                discussion5.AddMessage("Does this ring fit well? I'm worried the sizes are not accurate.");
                discussion5.AddMessage("Yes, my boyfriend ordered it in a size 17, which fits perfectly!");
                discussions.Add(discussion5);
            }

            if (discussions.Any())
            {
                await _context.Discussions.InsertManyAsync(discussions, cancellationToken: cancellationToken);
                _logger.LogInformation("Successfully seeded {Count} discussions", discussions.Count);
            }
        }
    }
}

