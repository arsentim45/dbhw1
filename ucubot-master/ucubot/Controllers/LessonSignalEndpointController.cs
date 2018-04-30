using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using ucubot.Model;
using Dapper;

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
            using (var conn = new MySqlConnection(connectionString))
            {
            	conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM lesson_signal", conn);
                
                var dataset = new DataSet();
                
                adapter.Fill(dataset, "lesson_signal");

                foreach (DataRow row in dataset.Tables[0].Rows)
                {
					var elem = new LessonSignalDto;
					elem = conn.Query<LessonSignalDto>(	
					"select lesson_signal.id as id, timestamp_ as Timestamp, signal_type as signalType, user_id as UserId from lesson_signal;");
                    yield return elem;
                }
            }
        }
        
        [HttpGet("{id}")]
        public LessonSignalDto ShowSignal(long id)
        {
            using (var conn = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                conn.Open();
                var command = new MySqlCommand("SELECT * FROM lesson_signal WHERE id = @id", conn);
                command.Parameters.AddWithValue("id", id);
                var adapter = new MySqlDataAdapter(command);
                
                var dataset = new DataSet();
                
                adapter.Fill(dataset, "lesson_signal");
                if (dataset.Tables[0].Rows.Count < 1)
                    return null;
                
                var row = dataset.Tables[0].Rows[0];
				var elem = new LessonSignalDto;
                elem = conn.Query<LessonSignalDto>(	
                "select lesson_signal.id as id, timestamp_ as Timestamp, signal_type as signalType, user_id as UserId from lesson_signal WHERE lesson_signal.id =@id;", new {Id = id});
                return elem;
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateSignal(SlackMessage message)
        {
            var userId = message.user_id;
            var signalType = message.text.ConvertSlackMessageToSignalType();

            using (var conn = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
				if 
                conn.Open();
                var command = conn.CreateCommand();
				var elem = new LessonSignalDto;
                elem = conn.Query<LessonSignalDto>("select id as Id, first_name as FirstName, last_name as LastName, user_id as UserId from lesson_signal where user_id=@uId");
				if(elem.Count == 0){
					return BadRequest();
				}
                command.CommandText =
                    "INSERT INTO lesson_signal (user_id, signal_type) VALUES (@userId, @signalType);";
                command.Parameters.AddRange(new[]
                {
                	new MySqlParameter("userId", userId),
                    new MySqlParameter("signalType", signalType)
                });
                await command.ExecuteNonQueryAsync();
				conn.Close();
            }
            
            return Accepted();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveSignal(long id)
        {
            using (var conn = new MySqlConnection(_configuration.GetConnectionString("BotDatabase")))
            {
                conn.Open();
                var command = conn.CreateCommand();
                command.CommandText =
                    "DELETE FROM lesson_signal WHERE ID = @id;";
            	command.Parameters.Add(new MySqlParameter("id", id));
                await command.ExecuteNonQueryAsync();
            }
            
            return Accepted();
        }
    }
}
