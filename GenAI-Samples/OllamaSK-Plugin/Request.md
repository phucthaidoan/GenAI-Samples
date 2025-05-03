# Inspect the requests
## This file is used to inspect the requests made by the OllamaSK plugin.

### Request 1
HTTP POST: http://localhost:11434/api/chat \
Payload:
```json
{
    "model": "llama3.1",
    "messages": [
        {
            "role": "user",
            "content": "What are all of the critical tasks?"
        }
    ],
    "options": {},
    "stream": false,
    "tools": [
        {
            "type": "function",
            "function": {
                "name": "TaskManagement-complete_task",
                "description": "Updates the status of the specified task to Completed",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "id": {
                            "type": "integer",
                            "description": null,
                            "enum": null
                        }
                    },
                    "required": [
                        "id"
                    ]
                }
            }
        },
        {
            "type": "function",
            "function": {
                "name": "TaskManagement-get_critical_tasks",
                "description": "Gets a list of all tasks marked as 'Critical' priority",
                "parameters": {
                    "type": "object",
                    "properties": {},
                    "required": null
                }
            }
        }
    ],
    "CustomHeaders": {}
}
```

Response:
```json
{
    "model": "llama3.1",
    "created_at": "2025-05-03T04:24:45.7061725Z",
    "message": {
        "role": "assistant",
        "content": "",
        "tool_calls": [
            {
                "function": {
                    "name": "TaskManagement-get_critical_tasks",
                    "arguments": {}
                }
            }
        ]
    },
    "done_reason": "stop",
    "done": true,
    "total_duration": 2028894000,
    "load_duration": 17989300,
    "prompt_eval_count": 212,
    "prompt_eval_duration": 699684200,
    "eval_count": 16,
    "eval_duration": 1310601000
}
```

### Request 2
HTTP POST: http://localhost:11434/api/chat \
Payload:
```json
{
    "model": "llama3.1",
    "messages": [
        {
            "role": "user",
            "content": "What are all of the critical tasks?"
        },
        {
            "role": "assistant",
            "content": null,
            "tool_calls": [
                {
                    "function": {
                        "index": null,
                        "name": "TaskManagement-get_critical_tasks",
                        "arguments": {}
                    }
                }
            ]
        },
        {
            "role": "tool",
            "content": "{\"CallId\":\"6793e114\",\"Result\":[{\"id\":2,\"title\":\"Fix login bug\",\"description\":\"Resolve the issue with login sessions timing out\",\"status\":\"To Do\",\"priority\":\"Critical\"}]}"
        }
    ],
    "options": {},
    "stream": false,
    "tools": [
        {
            "type": "function",
            "function": {
                "name": "TaskManagement-complete_task",
                "description": "Updates the status of the specified task to Completed",
                "parameters": {
                    "type": "object",
                    "properties": {
                        "id": {
                            "type": "integer",
                            "description": null,
                            "enum": null
                        }
                    },
                    "required": [
                        "id"
                    ]
                }
            }
        },
        {
            "type": "function",
            "function": {
                "name": "TaskManagement-get_critical_tasks",
                "description": "Gets a list of all tasks marked as 'Critical' priority",
                "parameters": {
                    "type": "object",
                    "properties": {},
                    "required": null
                }
            }
        }
    ],
    "CustomHeaders": {}
}
```

Response:
```json
{
    "model": "llama3.1",
    "created_at": "2025-05-03T04:24:49.3655204Z",
    "message": {
        "role": "assistant",
        "content": "Based on the tool call response, there is one critical task:\n\n1. Fix login bug: Resolve the issue with login sessions timing out (Status: To Do, Priority: Critical)"
    },
    "done_reason": "stop",
    "done": true,
    "total_duration": 3608233000,
    "load_duration": 22330000,
    "prompt_eval_count": 133,
    "prompt_eval_duration": 351299100,
    "eval_count": 38,
    "eval_duration": 3233499200
}
```