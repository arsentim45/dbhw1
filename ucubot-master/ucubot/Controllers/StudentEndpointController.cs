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
    public class StudentEndpointController : Controller
    {
        private readonly IConfiguration _configuration;

        public StudentEndpointController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IEnumerable<Student> ShowSignals()
        {
            var connectionString = _configuration.GetConnectionString("BotDatabase");
            using (var conn = new MySqlConnection(connectionString))
            {
            	conn.Open();
                var adapter = new MySqlDataAdapter("SELECT * FROM Student", conn);
                
                var dataset = new DataSet();
                
                adapter.Fill(dataset, "Student");

                foreach (DataRow row in dataset.Tables[0].Rows)
                {
					var elem = new LessonSignalDto;
					elem = conn.Query<LessonSignalDto>(	
					"select id as id, timestamp_ as Timestamp, signal_type as signalType, user_id as UserId from student join lesson_signal;");
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
                var command = new MySqlCommand("SELECT * FROM Student WHERE id = @id", conn);
                command.Parameters.AddWithValue("id", id);
                var adapter = new MySqlDataAdapter(command);
                
                var dataset = new DataSet();
                
                adapter.Fill(dataset, "Student");
                if (dataset.Tables[0].Rows.Count < 1)
                    return null;
                
                var row = dataset.Tables[0].Rows[0];
				var elem = new LessonSignalDto;
                elem = conn.Query<LessonSignalDto>(	
                "select id as id, timestamp_ as Timestamp, signal_type as signalType, user_id as UserId from student join lesson_signal on student.id = lesson_signal.student_id WHERE lesson_signal.id =@id;", new {Id = id});
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
                elem = conn.Query<LessonSignalDto>("select id as Id, first_name as FirstName, last_name as LastName, user_id as UserId from student where user_id=@uId");
				if(elem.Count == 0){
					return BadRequest();
				}
                command.CommandText =
                    "INSERT INTO student (first_name, last_name, user_id) VALUES (@firstname, @lastname, @userid);";
                command.Parameters.AddRange(new[]
                {
                	new MySqlParameter("first_name", firstname),
                    new MySqlParameter("last_name", lastname),
					new MySqlParameter("user_id", userid)
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
                    "DELETE FROM student WHERE ID = @id;";
            	command.Parameters.Add(new MySqlParameter("id", id));
                await command.ExecuteNonQueryAsync();
            }
            
            return Accepted();
        }
    }
}
