# yedaisler

## Config.json sample
```json
{
	"gui": {
		"color": {
			"fontReady": "#FFFFFFFF",
			"fontDoing": "#FF202020",
			"fontDone": "#FFFFFFFF",
			"backReady": "#A0FF0000",
			"backDoing": "#A0FFFF00",
			"backDone": "#A0202020"
		},
		
		"startupLocation": "bottom-left"
	},

	"todos": [
		{
			"name": "Aにログイン",
			"displayInBox": true,

			"ready": {
				"name": "Aにログインする",
				"mode": "exec",

				"action": {
					"name": "URLを開く1",
					"type": "openUrl",
					"openUrl": {
						"url": "https://www.google.co.jp/"
					},
					"data": ""
				},
				"notify": {
					"active": true
				},
				"block": {
					"shutdown": false,
					"sleep": false
				}
			},
			"doing": {
				"name": "Aからログアウトする",
				"mode": "exec",

				"action": {
					"name": "URLを開く2",
					"type": "openUrl",
					"openUrl": {
						"url": "https://www.google.co.jp/"
					},
					"data": ""
				},

				"block": {
					"shutdown": true,
					"sleep": false
				}
			},
			"done": {
			}
		},
		{
			"name": "Bにログイン",

			"ready": {
				"name": "Bにログインする",
				"mode": "exec",

				"action": {
					"name": "URLを開く3",
					"type": "openUrl",
					"openUrl": {
						"url": "https://www.google.co.jp/"
					},
					"data": ""
				},

				"block": {
					"shutdown": false,
					"sleep": false
				}
			}
		}
	]
}
```
