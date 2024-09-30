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
		}
	},

	"todos": [
		{
			"name": "Aにログイン",
			"displayInBox": true,

			"ready": {
				"name": "ログインする",
				"mode": "exec",

				"action": {
					"name": "URLを開く",
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
			},
			"doing": {
				"name": "ログアウトする",
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
				"name": "ログインする",
				"mode": "exec",

				"action": {
					"name": "URLを開く",
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
