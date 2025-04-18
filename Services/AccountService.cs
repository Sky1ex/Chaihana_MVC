using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Twilio.Rest.Api.V2010.Account.Sip.Domain.AuthTypes.AuthTypeCalls;
using WebApplication1.DataBase;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CartService> _logger;
        public static List<CodeDto> _codeList = new List<CodeDto>();

        public AccountService(ApplicationDbContext context, ILogger<CartService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<AddressDto>> GetAddresses(Guid userId)
        {
            var user = await _context.Users
                .Include(c => c.Adresses)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            List<AddressDto> Adresses = user.Adresses
                .Select(c => new AddressDto
                {
                    AddressId = c.AdressId,
                    City = c.City,
                    Street = c.Street,
                    House = c.House,
                    Apartment = c.Apartment
                }).ToList();

            return Adresses;
        }

        public List<OrderDto> GetOrders(Guid userId)
        {
            var user = _context.Users
                .Include(c => c.Orders)
                .ThenInclude(ce => ce.OrderElement)
                .ThenInclude(ced => ced.Product)
                .Include(c => c.Orders)
                .ThenInclude(ce => ce.Adress)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            var orders = user.Result.Orders
                .Select(c => new OrderDto
                {
                    DateTime = c.dateTime,
                    Address = new AddressDto
                    {
                        City = c.Adress.City,
                        Street = c.Adress.Street,
                        House = c.Adress.House,
                        Apartment = c.Adress.Apartment
                    },
                    Products = c.OrderElement.Select(ce => new OrderProductDto
                    {
                        Name = ce.Product.Name,
                        Price = ce.Product.Price,
                        Count = ce.Count
                    }).ToList()
                })
                .ToList();

            return orders;
        }

        public async Task<HttpResponseMessage> PostSms(string userNumber, Guid userId)
        {
            Random rand = new Random();

            string randomCode = "";
            for (int i = 0; i < 6; i++)
            {
                randomCode += (char)rand.Next(97, 122);
            }

            HttpClient httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "eyJhbGciOiJSUzI1NiIsInR5cCIgOiAiSldUIiwia2lkIiA6ICJRV05sMENiTXY1SHZSV29CVUpkWjVNQURXSFVDS0NWODRlNGMzbEQtVHA0In0.eyJleHAiOjIwNTc2MTQ3ODEsImlhdCI6MTc0MjI1NDc4MSwianRpIjoiNWYyOGFlNzAtZmVhNy00ZGNkLTk4NzMtM2YxZGU3MjY3YjkwIiwiaXNzIjoiaHR0cHM6Ly9zc28uZXhvbHZlLnJ1L3JlYWxtcy9FeG9sdmUiLCJhdWQiOiJhY2NvdW50Iiwic3ViIjoiYWUyM2YwNDItODMwNy00NzBmLTliNDAtNGJmZjFmZTcxZWY1IiwidHlwIjoiQmVhcmVyIiwiYXpwIjoiNDU0NDczMmQtNDcwMS00YzBiLTgwMDUtMjUxZGFlMTQwOTc1Iiwic2Vzc2lvbl9zdGF0ZSI6ImE0NWU4YmVkLTg3ZmYtNGNjZi1iZmUyLTczNTg0MDNiMGI5MCIsImFjciI6IjEiLCJyZWFsbV9hY2Nlc3MiOnsicm9sZXMiOlsiZGVmYXVsdC1yb2xlcy1leG9sdmUiLCJvZmZsaW5lX2FjY2VzcyIsInVtYV9hdXRob3JpemF0aW9uIl19LCJyZXNvdXJjZV9hY2Nlc3MiOnsiYWNjb3VudCI6eyJyb2xlcyI6WyJtYW5hZ2UtYWNjb3VudCIsIm1hbmFnZS1hY2NvdW50LWxpbmtzIiwidmlldy1wcm9maWxlIl19fSwic2NvcGUiOiJleG9sdmVfYXBwIHByb2ZpbGUgZW1haWwiLCJzaWQiOiJhNDVlOGJlZC04N2ZmLTRjY2YtYmZlMi03MzU4NDAzYjBiOTAiLCJ1c2VyX3V1aWQiOiJmNzc0ZjA1OS0xNTJjLTQwOGMtYjhkZi1lOWJhOTZiZjVkZDYiLCJjbGllbnRJZCI6IjQ1NDQ3MzJkLTQ3MDEtNGMwYi04MDA1LTI1MWRhZTE0MDk3NSIsImVtYWlsX3ZlcmlmaWVkIjpmYWxzZSwiY2xpZW50SG9zdCI6IjE3Mi4xNi4xNjEuMTkiLCJhcGlfa2V5Ijp0cnVlLCJhcGlmb25pY2Ffc2lkIjoiNDU0NDczMmQtNDcwMS00YzBiLTgwMDUtMjUxZGFlMTQwOTc1IiwiYmlsbGluZ19udW1iZXIiOiIxMjk5NTE0IiwiYXBpZm9uaWNhX3Rva2VuIjoiYXV0MjIyNzc3YTQtY2JiNy00M2NiLThhOWYtNTgzMzBhMzlmNDg3IiwicHJlZmVycmVkX3VzZXJuYW1lIjoic2VydmljZS1hY2NvdW50LTQ1NDQ3MzJkLTQ3MDEtNGMwYi04MDA1LTI1MWRhZTE0MDk3NSIsImN1c3RvbWVyX2lkIjoiMTA0NDkzIiwiY2xpZW50QWRkcmVzcyI6IjE3Mi4xNi4xNjEuMTkifQ.KBDgjrEhuE1teUcWKOTn2YyiFfHKPA4vYNmU8kDbnJk8p9BwbS48gygfH-Ksf2eoWxePfZ8Z80tCTVSm1zxqNSBZnmj22cCjb-DqLLHt2tTrnyqv6sS4KY-CefZGSZaUmRQPoBUbq4rNJHqaPjPGAQsqFwNrPrQaXuXPOqviD8fwe9FMQ2aRG0aHWIJrt5wbmFSA1jNWttdemWbNL_4THYI40D4lmm0Nz7s2nvREHmKY2iI2ChanmTpwCo8f66mf2cMOZBVka4X6M1iL4Xf8dUJ8h-ArVV5hGi3BT0xlCScHwWf6T8Pj2MVHZwm5OpWufEB0G_CTsBti5HQYa5dgIA");
            PhoneRequestDto phone = new PhoneRequestDto { number = "79587109048", destination = userNumber, text = $"{randomCode}" };
            var json = JsonSerializer.Serialize(phone);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var response = await httpClient.PostAsync("https://api.exolve.ru/messaging/v1/SendSMS", content);

            if (response.IsSuccessStatusCode)
            {
                CodeDto codeDto = _codeList.FirstOrDefault(x => x.UserId == userId);
                if (codeDto == null)
                {
                    _codeList.Add(new CodeDto { Code = randomCode, UserId = userId, number = userNumber });
                }
                else codeDto.Code = randomCode;
            }

            return response;
        }

        public async Task<string> CheckCode(string code, Guid userId)
        {
            CodeDto codeDto = _codeList.FirstOrDefault( x => x.Code == code && x.UserId == userId);
            if (codeDto == null) return "false";
            else
            {
                var checkUser = await _context.Users.FirstOrDefaultAsync(x => x.Phone == codeDto.number);
                if (checkUser != null)
                {
                    var user = await _context.Users
                        .FirstOrDefaultAsync(c => c.UserId == userId);

                    _context.Users.Remove(user);
                    await _context.SaveChangesAsync();
                    _codeList.Remove(codeDto);
                    return checkUser.UserId.ToString();
                }
                else return "true";
            }
        }

        public async Task<bool> AddName(string name, Guid userId)
        {
            CodeDto codeDto = _codeList.FirstOrDefault(x => x.UserId == userId);
            var user = await _context.Users
             .FirstOrDefaultAsync(c => c.UserId == userId);

            user.Name = name;
            user.Phone = codeDto.number;

            await _context.SaveChangesAsync();
            _codeList.Remove(codeDto);

            return true;
        }

        public async Task<bool> AddAddress(string City, string Street, string House, int Apartment, Guid userId)
        {
            var user = await _context.Users
                .Include(c => c.Adresses)
                .FirstOrDefaultAsync(c => c.UserId == userId);
            var address = new Adress { AdressId = Guid.NewGuid(), City = City, House = House, Street = Street, Apartment = Apartment };
            user.Adresses.Add(address);
            await _context.Adresses.AddAsync(address);
            await _context.SaveChangesAsync();
            return true;
        }

		public async Task<bool> PutAddress(string City, string Street, string House, int Apartment, Guid userId, Guid addressId)
		{
			var user = await _context.Users
				.Include(c => c.Adresses)
				.FirstOrDefaultAsync(c => c.UserId == userId);
            var address = user.Adresses.FirstOrDefault(c => c.AdressId == addressId);
            if(address == null) return false;
            address.House = House;
            address.City = City;
            address.Street = Street;
            address.Apartment = Apartment;
			await _context.SaveChangesAsync();
			return true;
		}

		public async void DeleteAddress(string addressId, Guid userId)
        {
            var user = _context.Users
                .Include(c => c.Adresses)
                .FirstOrDefault(c => c.UserId == userId);

            var address = user.Adresses.FirstOrDefault(c => c.AdressId == new Guid(addressId));


            user.Adresses.Remove(address);
            _context.Adresses.Remove(address);
            await _context.SaveChangesAsync();
        }
    }
}

