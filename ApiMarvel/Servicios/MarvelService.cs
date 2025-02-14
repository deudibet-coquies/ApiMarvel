using System.Text;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Security.Cryptography;
using ApiMarvel.Modelos.Dto;
using ApiMarvel.Modelos;
using ApiMarvel.Servicios.IServicios;
using ApiMarvel.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using XAct.Users;
using System.Security.Claims;
using XAct.Library.Settings;
using Microsoft.EntityFrameworkCore;
using static ApiMarvel.Servicios.MarvelService;
using System.Text.Json;
using Microsoft.Extensions.Options;

namespace ApiMarvel.Servicios
{
    public class MarvelService : IMarvelService
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly Appsettings _appsettings;
        public MarvelService(ApplicationDbContext applicationDbContext, IOptions<Appsettings> appsettings)
        {
            _applicationDbContext = applicationDbContext;
            _appsettings = appsettings.Value;
        }
       
        public async Task<ICollection<ComicDto>> GetComicsAsync()
        {
            using (var client = new HttpClient())
            {
                string timestamp = DateTime.Now.Ticks.ToString();
                string hash = GenerateHash(timestamp);

                var url = $"{_appsettings.apiUrl}?ts={timestamp}&apikey={_appsettings.PublicKey}&hash={hash}";

                var response = await client.GetStringAsync(url);
                var result = JsonConvert.DeserializeObject<MarvelResponseDto>(response);

                return result.Data.Results;
            }
        }

        private  string GenerateHash(string timestamp)
        {
            using (MD5 md5 = MD5.Create())
            {
                string hashInput = timestamp + _appsettings.PrivateKey + _appsettings.PublicKey;
                byte[] hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(hashInput));
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }

        // todo el tema de favoritos

        public Task<ICollection<Favorito>> GetFavoritos(string usuarioId)
        {
       
            return Task.FromResult<ICollection<Favorito>>(_applicationDbContext.Favoritos
                .Where(f => f.IdUsuario == usuarioId)
                .ToList());
        }


        public async Task<Favorito> GetFavoritoPorUsuarioYComic(string userId, string comicId)
        {
            return await _applicationDbContext.Favoritos.FirstOrDefaultAsync(f => f.IdUsuario == userId && f.IdComics == comicId);
        }

        public async Task<bool> EliminarFavorito(Favorito favorito)
        {
            _applicationDbContext.Favoritos.Remove(favorito);
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }

        public async Task<bool> AddFavorito(Favorito favorito)
        {
            await _applicationDbContext.Favoritos.AddAsync(favorito);
            return await _applicationDbContext.SaveChangesAsync() > 0;
        }

        public async Task<ComicDto> GetComicId(string comicId)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string timestamp = DateTime.Now.Ticks.ToString();
                    string hash = GenerateHash(timestamp);

                    var url = $"{_appsettings.apiUrl}/{comicId}?ts={timestamp}&apikey={_appsettings.PublicKey}&hash={hash}";
                    var response = await client.GetAsync(url);

                    if (!response.IsSuccessStatusCode)
                    {
                        return null;
                    }

                    var jsonString = await response.Content.ReadAsStringAsync();
                    using var document = JsonDocument.Parse(jsonString);
                    var root = document.RootElement;

                    var results = root.GetProperty("data").GetProperty("results");
                    if (results.GetArrayLength() == 0)
                    {
                        return null;
                    }

                    var comicData = results[0];

                    return new ComicDto
                    {
                        id = comicData.GetProperty("id").GetInt32(),
                        Title = comicData.GetProperty("title").GetString(),
                        Description = comicData.TryGetProperty("description", out var desc) ? desc.GetString() ?? "Sin descripción." : "Sin descripción.",
                        thumbnail = new Thumbnail
                        {
                            path = comicData.GetProperty("thumbnail").GetProperty("path").GetString(),
                            extension = comicData.GetProperty("thumbnail").GetProperty("extension").GetString()
                        }
                    };
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener cómic {comicId}: {ex.Message}");
                return null;
            }
        }




    }


}