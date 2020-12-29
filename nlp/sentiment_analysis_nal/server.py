import flask
from flask import request, jsonify
import pickle
import train

app = flask.Flask(__name__)
app.config["DEBUG"] = True

MODEL = "model.sav"
loaded_model = pickle.load(open(MODEL, 'rb'))

@app.route('/api/predict', methods=['GET'])
def predict():
    try:
        if 'msg' in request.args:
            msg = str(request.args['msg'])
        else:
            return "ERROR: The field 'msg' is required.", 400

        result = ''
        msg = train.normalize_text(msg)
        result = loaded_model.predict([str(msg)])
        return str(result[0]), 200
    except:
        return 'ERROR: Internal Server', 500

if __name__ == '__main__':
    app.run(host='0.0.0.0',port=9010)
