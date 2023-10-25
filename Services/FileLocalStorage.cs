namespace ApiPeliculas.Services
{
    public class FileLocalStorage : IFileStorage
    {
        private readonly IWebHostEnvironment env;  // path wwwroot
        private readonly IHttpContextAccessor httpContextAccessor;  // domain webapi

        public FileLocalStorage(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
        {
            this.env = env;
            this.httpContextAccessor = httpContextAccessor;
        }

        public IWebHostEnvironment Env { get; }

        public Task DeleteFile(string ruta, string contenedor)
        {
            if (ruta != null)
            {
                var fileName = Path.GetFileName(ruta);
                string fileDirectory = Path.Combine(env.WebRootPath, fileName);

                if (File.Exists(fileDirectory)) { File.Delete(fileDirectory); }
            }

            return Task.FromResult(0);
        }

        public async Task<string> EditFile(byte[] contenido, string extension, string contenedor, string ruta, string contentType)
        {
            await DeleteFile(ruta, contenedor);
            return await SaveFile(contenido, extension, contenedor, contentType);
        }

        public async Task<string> SaveFile(byte[] contenido, string extension, string contenedor, string contentType)
        {
            var fileName = $"{Guid.NewGuid()}{extension}";
            string folder = Path.Combine(env.WebRootPath, contenedor);

            if(!Directory.Exists(folder)) { Directory.CreateDirectory(folder); }

            string path = Path.Combine(folder, fileName);
            await File.WriteAllBytesAsync(path, contenido);

            var actualPath = $"{httpContextAccessor.HttpContext.Request.Scheme}://{httpContextAccessor.HttpContext.Request.Host}";
            var dbPath = Path.Combine(actualPath, contenedor, fileName).Replace("\\", "/");

            return dbPath;
        }
    }
}
