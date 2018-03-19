using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;

namespace ucubot.Controllers
{
    [Route("api/[controller]")]
    public class LessonSignalEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public LessonSignalEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<LessonSignalDto> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get all signals
            var conn = new MySqlConnection(connectionString); 
            var dataTable = new DataTable();
            var adapt = new MySqlDataAdapter("SELECT * FROM lesson_signal", conn);
            conn.Open();
            adapt.Fill(dataTable);
            conn.Close();
            var lst = new List<LessonSignalDto>();
            foreach (DataRow row in dataTable.Tables[0].Rows){
                    var elem = new LessonSignalDto{
                        Id = (int) row["id"],
                        Timestamp = (DateTime) row["timestamp1"],
                        Type = (LessonSignalType) row["SignalType"],
                        UserId = (string) row["UserId"]
                    };
                    lst.add(elem);
                }
            return lst;
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add query to get a signal by the given id
            var conn = new MySqlConnection(connectionString);
            var adapt = new MySqlDataAdapter("SELECT * FROM lesson_signal WHERE id = @id",conn);
            command.Parameters.AddWithValue("id",id);
            var dataTable = new DataTable();
            conn.Open();
            adapt.Fill(dataTable);
            conn.Close();
            var row = dataTable.Rows[0];
            var elem = new LessonSignalDto{
                Timestamp = (DateTime) row["timestamp1"],
                Type = (LessonSignalType) row["SignalType"],
                UserId = (string) row["UserId"]
            };
            if(dataTable.Rows.Count >= 0){
                return elem;
            }
            return null;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            // TODO: add insert command to store signal
            var com = "INSERT INTO lesson_signal (UserId, SignalType) VALUES (@userId, @signalType);";
            var conn = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand(com, conn);
            cmd.Parameters.AddWithValue("userId",userId);
            cmd.Parameters.AddWithValue("signalType", signalType);
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {   
            var com =  "DELETE FROM lesson_signal WHERE id=@id";
            var conn = new MySqlConnection(connectionString);
            var cmd = new MySqlCommand(com, conn);
            cmd.Parameters.AddWithValue("id", id);
            //TODO: add delete command to remove signal
            conn.Open();
            cmd.ExecuteNonQuery();
            conn.Close();
            return Accepted();
        }
    }
}
