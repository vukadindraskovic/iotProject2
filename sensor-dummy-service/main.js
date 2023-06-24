const mqtt = require('mqtt')

const clientId = 'sensor_dummy'
const username = 'sensor_dummy'
const password = 'sensor_dummy'
const topic = 'sensor_dummy/values'
const qos = 0

let i = 0
data = []

const address = 'broker.emqx.io'

const client = mqtt.connect('mqtt://broker.emqx.io:1883', {
    clientId,
    username, 
    password
})

const fs = require('fs')
const { parse } = require('csv-parse')
const parser = parse({columns: true}, function (err, records) {
    data = records
    setInterval(sendRow, 1000);
});

fs.createReadStream('./data.csv').pipe(parser)

function sendRow() {
    data[i].temp = parseInt(data[i].temp)
    client.publish(topic, JSON.stringify(data[i]), { qos }, error => {
        if (error) {
            console.error('ERROR: ', error)
        }
    })
    i = (i + 1) % data.length
}