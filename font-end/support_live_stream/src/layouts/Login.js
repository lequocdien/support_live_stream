import React from 'react';
import { Form, Input, Button, Row, Col, Card } from 'antd';
import { toast, ToastContainer } from 'react-toastify';
import { useHistory } from 'react-router-dom';

const layout = {
    labelCol: { span: 4 },
    wrapperCol: { span: 20 },
};
const tailLayout = {
    wrapperCol: { offset: 4, span: 20 },
};

const Login = () => {
    const history = useHistory();

    const HttpPostLogin = (username, password) => {
        return new Promise((resolve, reject) => {
            fetch(`${process.env.REACT_APP_API_HOST}/oauth/login`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify({
                    "username": username,
                    "password": password
                })
            }).then(resp => {
                if (resp.status === 200) {
                    return resp.json();
                }
                return reject("Đăng nhập thất bại.");
            }).then(data => {
                resolve(data);
            }).catch(err => reject("ERROR: " + err))
        });
    }

    const handleLogin = values => {
        HttpPostLogin(values.username, values.password)
            .then(data => {
                localStorage.clear();
                localStorage.setItem("access-token", data.accessToken);
                localStorage.setItem("isReady", data.isReady);
                localStorage.setItem("userName", data.userName);
                localStorage.setItem("role", data.role);

                toast.success("Đăng nhập thành công.");
                data.role === "ADMIN" ? history.push("/manageruser") : history.push("/config")
            }).catch(err => {
                toast.error("Đăng nhập thất bại.");
            })
    };

    const onFinishFailed = errorInfo => {
        console.log('Failed:', errorInfo);
    };

    return (
        <React.Fragment>
            <Row justify='center' style={{
                height: '100%',
                backgroundImage: "url(bg.png)",
                backgroundRepeat: "no-repeat",
                backgroundPosition: "center",
                backgroundSize: "cover"
            }}>
                <Col xs={24} md={10}
                    style={{ margin: "auto" }}
                >
                    <Card
                        title="Đăng nhập"
                        style={{
                            textAlign: "center"
                        }}
                    >
                        <Form
                            {...layout}
                            name="basic"
                            initialValues={{ remember: true }}
                            onFinish={handleLogin}
                            onFinishFailed={onFinishFailed}
                        >
                            <Form.Item
                                labelCol={{ span: 24 }}
                                wrapperCol={{ span: 24 }}
                                labelAlign="left"
                                label="Tên đăng nhập"
                                name="username"
                                rules={[{ required: true, message: 'Vui lòng nhập tên đăng nhập!' }]}
                            >
                                <Input autoComplete={false} value="admin" />
                            </Form.Item>

                            <Form.Item
                                labelCol={{ span: 24 }}
                                wrapperCol={{ span: 24 }}
                                labelAlign="left"
                                label="Mật khẩu"
                                name="password"
                                rules={[{ required: true, message: 'Vui lòng nhập mật khẩu!' }]}
                            >
                                <Input.Password autoComplete={false} value="admin" />
                            </Form.Item>

                            {/* <Form.Item {...tailLayout} name="remember" valuePropName="checked">
                                <Checkbox>Nhớ mật khẩu của tôi</Checkbox>
                            </Form.Item> */}

                            <Form.Item {...tailLayout}
                                labelCol={{ span: 24 }}
                                wrapperCol={{ span: 24 }}
                                style={{ textAlign: "center" }}
                            >
                                <Button type="primary" htmlType="submit">Đăng nhập</Button>
                            </Form.Item>
                        </Form>
                    </Card>
                </Col>
            </Row>
            <ToastContainer />
        </React.Fragment>
    );
};

export default Login;