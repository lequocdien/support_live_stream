# -*- coding: utf-8 -*-
import redis
import time
import pickle

MODEL = "model.sav"
HOST = "103.116.104.156"
POST = 6379
PASSWORD = "*Dien1234567890*"

loaded_model = pickle.load(open(MODEL, 'rb'))

r = redis.Redis(host=HOST, port=POST, password=PASSWORD, decode_responses=True)

pub_sub = r.pubsub()
pub_sub.subscribe("channel_1234567890_0")
for message in pub_sub.listen():
    if message and message['type'] and message['type'] == 'message':
        message = str(message['data'])
        y_predict = loaded_model.predict([message])
        r.publish("channel_1234567890_1", str(y_predict[0]))
        time.sleep(0.001)