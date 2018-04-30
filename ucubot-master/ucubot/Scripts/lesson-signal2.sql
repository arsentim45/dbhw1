use ucubot;
alter table lesson_signal drop user_id;
alter table lesson_signal add student_id varchar(55);
alter table lesson_signal add
	constraint new_student_id
	foreign key (student_id) references student(user_id)
	on update restrict on delete restrict;