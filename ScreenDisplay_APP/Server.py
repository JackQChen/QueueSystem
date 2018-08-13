import os
import json

from http.server import BaseHTTPRequestHandler, HTTPServer
class myClass: 
  def __init__(self): 
                self.name= ''
                self.sex= ''

class HTTPHandler(BaseHTTPRequestHandler):
        def do_GET(self):
                print('URL='+self.path)
                data = myClass()
                data.sex = 'female'
                self.wfile.write(json.dumps(data,default=lambda obj:obj.__dict__).encode('utf-8'))
        def do_POST(self):       
                self.send_response(200)
                self.end_headers()
                print('URL='+self.path)
                datas = self.rfile.read(int(self.headers['content-length']))
                print(datas)
                
                data = myClass()
                data.sex = 'female'
                byteData = json.dumps(data,default=lambda obj:obj.__dict__).encode('utf-8') 
                
                self.wfile.write(byteData)
try:
    print('server is running...')
    server_address = ('', 8000)
    http = HTTPServer(server_address, HTTPHandler)
    http.serve_forever()
except Exception as ex:
	print("Exception->" + str(ex))
os.system("pause");
