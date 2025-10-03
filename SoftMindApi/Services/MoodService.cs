using SoftMindApi.Entities;
using SoftMindApi.Repositories.Interface;
using SoftMindApi.Services.Interface;

namespace SoftMindApi.Services
{
    public class MoodService : IMoodService
    {
        private readonly IMoodRepository _moodRepository;
        private readonly IUserRepository _userRepository;

        public MoodService(IMoodRepository moodRepository, IUserRepository userRepository)
        {
            _moodRepository = moodRepository;
            _userRepository = userRepository;
        }

        public async Task<List<Mood>> GetMoodLastSevenDaysAsync(string deviceId)
        {
            return await _moodRepository.GetLastSevenDaysAsync(deviceId);
        }

        public async Task<Mood> AddMoodAsync(string deviceId, string emojiName)
        {
            var user = await _userRepository.GetByDeviceIdAsync(deviceId);
            if (user == null)
            {
                user = new User { DeviceId = deviceId };
                await _userRepository.AddAsync(user);
                await _userRepository.SaveChangesAsync();
            }

            var newMood = new Mood
            {
                Name = emojiName,
                DeviceId = user.DeviceId,
                Data = DateTime.Now
            };

            await _moodRepository.AddAsync(newMood);
            await _moodRepository.SaveChangesAsync();

            return newMood;
        }
    }
}
