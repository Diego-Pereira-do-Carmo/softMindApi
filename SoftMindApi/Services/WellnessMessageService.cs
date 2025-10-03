using SoftMindApi.DTO;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Services
{
    public class WellnessMessageService : IWellnessMessageService
    {
        private readonly IWellnessMessageRepository _repository;

        public WellnessMessageService(IWellnessMessageRepository repository)
        {
            _repository = repository;
        }

        private static TimeZoneInfo GetBrazilTimeZone()
        {
            try { return TimeZoneInfo.FindSystemTimeZoneById("America/Sao_Paulo"); } catch { }
            try { return TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"); } catch { }
            return TimeZoneInfo.Local;
        }

        public async Task<List<WellnessMessageDTO>> GetRandomForDeviceAsync(string deviceId, int take = 5)
        {
            var deviceKey = deviceId.Trim().ToLowerInvariant();

            var tz = GetBrazilTimeZone();
            var nowBr = TimeZoneInfo.ConvertTime(DateTime.Now, tz);

            var allActive = await _repository.GetAllActiveAsync();
            if (allActive.Count == 0)
            {
                return new List<WellnessMessageDTO>();
            }

            var random = new Random();
            var chosenList = allActive
                .OrderBy(_ => random.Next())
                .Take(take)
                .ToList();

            foreach (var chosen in chosenList)
            {
                var readStat = chosen.ReadStats.FirstOrDefault(s => s.DeviceId == deviceKey);
                if (readStat == null)
                {
                    chosen.ReadStats.Add(new Entities.WellnessReadStat
                    {
                        DeviceId = deviceKey,
                        Count = 1,
                        LastReadAt = nowBr
                    });
                }
                else
                {
                    readStat.Count += 1;
                    readStat.LastReadAt = nowBr;
                }
            }

            await _repository.SaveChangesAsync();

            return chosenList
                .Select(c => new WellnessMessageDTO { Id = c.Id.ToString(), Name = c.Name })
                .ToList();
        }
    }
}
