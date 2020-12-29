import { Breadcrumb, Button, Col, Layout, Row, Spin, Table } from 'antd';
import React, { useEffect, useState } from 'react';
import { toast, ToastContainer } from 'react-toastify';

const token = localStorage.getItem("access-token");
const pageId = localStorage.getItem("pageId");

//#region  HttpRequest
const HttpGetBlackList = (pageId) => {
    return new Promise((resolve, reject) => {
        if (pageId) {
            fetch(`${process.env.REACT_APP_API_HOST}/page/block?pageId=${pageId}`, {
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
        }
    });
}

const HttpPostUnblockUser = (pageId, blockId) => {
    return new Promise((resolve, reject) => {
        if (pageId) {
            fetch(`${process.env.REACT_APP_API_HOST}/page/block?pageId=${pageId}&blockId=${blockId}`, {
                method: 'DELETE',
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
                reject("ERROR: " + err);
            });
        }
    });
}
//#endregion

const BlackList = () => {
    const columns = [
        {
            key: 1,
            title: "Tên",
            dataIndex: "name",
            width: 350,
            align: "center"
        },
        {
            key: 1,
            title: "Hành đông",
            width: 350,
            align: "center",
            render: (value) => {
                return (
                    <Button type="primary"
                        onClick={() => {
                            setLoading(true);
                            HttpPostUnblockUser(pageId, value.id)
                                .then(() => { toast.success("Hủy chặn thành công."); setLoading(false); setIsReload(!isReload) })
                                .catch(() => { toast.error("Thêm vào sổ đen thất bại."); setLoading(false) })
                        }}
                    >Hủy chặn</Button>
                )
            }
        }
    ]

    const [loading, setLoading] = useState(false);
    const [blackList, setBlackList] = useState([]);
    const [isReload, setIsReload] = useState(false);

    useEffect(() => {
        setLoading(true);
        HttpGetBlackList(pageId).then(res => {
            setBlackList(res.data);
            setLoading(false);
        }).catch(err => console.log(`BlackList => ${err}`));
    }, [isReload]);

    return (
        <React.Fragment>
            <Layout style={{ height: "100%" }} >
                <Row>
                    <Col>
                        <Breadcrumb className="App-content-header">
                            <Breadcrumb.Item>Sổ đen</Breadcrumb.Item>
                        </Breadcrumb>
                    </Col>
                </Row>

                <Row>
                    <Col style={{ width: "100%" }}>
                        <Table
                            style={{ textAlign: "center" }}
                            loading={loading}
                            dataSource={blackList}
                            columns={columns}
                            bordered
                            pagination={{ defaultPageSize: 10 }}
                            scroll={{ x: 200 }}
                        />
                    </Col>
                </Row>
            </Layout>

            <ToastContainer />
        </React.Fragment>
    );
};

export default BlackList;