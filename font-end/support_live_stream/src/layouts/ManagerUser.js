import { Breadcrumb, Card, Col, Layout, Row, Spin, Button, Table, Tag } from 'antd';
import React, { useEffect, useState } from 'react';
import { toast, ToastContainer } from 'react-toastify';

const ManagerUser = () => {

    const token = localStorage.getItem("access-token");

    const [rootLoading, setRootLoading] = useState(false);
    const [users, setUsers] = useState([]);

    const columns = [
        {
            title: 'Tên đăng nhập',
            dataIndex: 'username',
            align: "center",
            key: 1,
        },
        {
            title: 'Họ và tên',
            dataIndex: 'fullname',
            key: 2,
            align: "center",
            render: (val, record) => {
                return `${record.firstName} ${record.lastName}`;
            }
        },
        {
            title: 'Địa chỉ',
            dataIndex: 'address',
            key: 3,
            align: "center",
        },
        {
            title: 'Trạng thái',
            dataIndex: 'isActive',
            key: 4,
            align: "center",
            render: (val) => {
                return val ? <Tag color="#1890ff">Đang hoạt động</Tag> : <Tag color="#ff4d4f" >Ngưng hoạt động</Tag>
            }
        },
        {
            title: 'Hành động',
            key: 5,
            align: "center",
            render: (val, record) => {
                return (
                    <Button key={record.id.timestamp} type="primary" danger={record.isActive ? true : false}
                        onClick={() => {
                            HttpPostUser(record.username, record.role, !record.isActive).then((data) => toast.success(data)).catch(err => toast.error(err));
                        }}
                    >{record.isActive ? "Khóa" : "Kích hoạt"}</Button>
                )
            }
        },
    ];

    //#region HttpRequest
    const HttpGetUser = () => {
        return new Promise((resolve, reject) => {
            fetch(`${process.env.REACT_APP_API_HOST}/account/get`, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json',
                    'access_token': token
                },
            }).then(response => {
                if (response.status !== 200) {
                    return reject(response);
                }
                return response.json();
            }).then(data => {
                resolve(data);
            }).catch((err) => {
                reject("ERROR: " + err)
            });
        });
    }

    const HttpPostUser = (username, role, isActive) => {
        return new Promise((resolve, reject) => {
            setRootLoading(true);
            fetch(`${process.env.REACT_APP_API_HOST}/account/update?userName=${username}&role=${role}&isActive=${isActive}`, {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'access_token': token
                },
                body: JSON.stringify({})
            }).then(response => {
                if (response.status !== 200) {
                    return reject("Cập nhật thất bại!");
                }
                return HttpGetUser();
            }).then((data) => {
                setRootLoading(false);
                setUsers(data);
                return resolve("Cập nhật thành công!");
            }).catch(() => {
                setRootLoading(false);
                return reject("Cập nhật thất bại!");
            });
        });
    }
    //#endregion

    useEffect(() => {
        HttpGetUser().then(data => setUsers(data)).catch(err => console.log(err));
    }, []);

    return (
        <React.Fragment>
            <ToastContainer />
            <Spin spinning={rootLoading} >
                <Layout style={{ height: "100%" }} >
                    <Row>
                        <Col>
                            <Breadcrumb className="App-content-header">
                                <Breadcrumb.Item>Quản lý User</Breadcrumb.Item>
                            </Breadcrumb>
                        </Col>
                    </Row>
                    <Row justify="center">
                        <Col xs={24} lg={24}>
                            <Card>
                                <Table
                                    dataSource={users}
                                    columns={columns} />
                            </Card>
                        </Col>
                    </Row>
                </Layout>
            </Spin>
        </React.Fragment>
    );
};

export default ManagerUser;