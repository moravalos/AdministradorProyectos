using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Security.Cryptography;
using System.Web;

using CRUDCORE.Datos;
using CRUDCORE.Models;


namespace CRUDCORE.Controllers
{
    public class AccessController : Controller
    {
        ContactoDatos _ContactoDatos = new ContactoDatos();

        static string cadena = "Data Source=LAPTOP-DQEPM0TK\\SQLEXPRESS;Initial Catalog=DBCRUDCORE;Integrated Security=True";
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Registrar()
        {
            //METODO SOLO DEVUELVE LA VISTA
            return View();
        }

        [HttpPost]

        public IActionResult Registrar(Usuario Usuario)
        {
            //METODO RECIBE EL OBJETO PARA GUARDARLO EN BD
            if (!ModelState.IsValid)
                return View();


            var respuesta = _ContactoDatos.Registrar(Usuario);

            if (respuesta)
                return RedirectToAction("Listar","Mantenedor");
            else
                return View();
        }


        [HttpPost]
        public ActionResult Login(Usuario oUsuario)
        {
            oUsuario.Clave = ConvertirSha256(oUsuario.Clave);

            using (SqlConnection cn = new SqlConnection(cadena))
            {

                SqlCommand cmd = new SqlCommand("sp_ValidarUsuario", cn);
                cmd.Parameters.AddWithValue("Correo", oUsuario.Correo);
                cmd.Parameters.AddWithValue("Clave", oUsuario.Clave);
                cmd.CommandType = CommandType.StoredProcedure;

                cn.Open();

                oUsuario.IdUsuario = Convert.ToInt32(cmd.ExecuteScalar().ToString());

            }

            if (oUsuario.IdUsuario != 0)
            {
                return RedirectToAction("Listar", "Mantenedor");
            }
            else
            {
                ViewData["Mensaje"] = "usuario no encontrado";
                return View();
            }


        }



        public static string ConvertirSha256(string texto)
        {
            //using System.Text;
            //USAR LA REFERENCIA DE "System.Security.Cryptography"

            StringBuilder Sb = new StringBuilder();
            using (SHA256 hash = SHA256Managed.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(texto));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }



    }
}
