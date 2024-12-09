#!/bin/bash
rabbitmqctl add_user user_api api123
rabbitmqctl set_permissions -p / user_api ".*" ".*" ".*"

rabbitmqctl add_user task_api api123
rabbitmqctl set_permissions -p / task_api ".*" ".*" ".*"

rabbitmqctl status